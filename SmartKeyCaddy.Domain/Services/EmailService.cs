using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Azure.Communication.Email;
using HotelCheckIn.Domain.Contracts;
using SmartKeyCaddy.Models;
using SmartKeyCaddy.Models.Configurations;
using SmartKeyCaddy.Domain.Contracts;
using SmartKeyCaddy.Common;

namespace SmartKeyCaddy.Domain.Services;

public partial class EmailService : IEmailService
{
    private readonly EmailApiSettings _emailApiSettings;
    private readonly QrCodeSettings _qrCodeApiSettings;
    private readonly ILogger<EmailService> _logger;
    private readonly ITemplateService _templateService;

    public EmailService(
        IOptions<QrCodeSettings> qrCodeApiSettings,
        IOptions<EmailApiSettings> emailApiSettings,
        ILogger<EmailService> logger,
        ITemplateService templateService)
    {
        _emailApiSettings = emailApiSettings.Value;
        _qrCodeApiSettings = qrCodeApiSettings.Value;
        _logger = logger;
        _templateService = templateService;
    }

    public async Task<bool> SendEmail(KeyAllocation keyAllocation, Reservation reservation, Property property, CommunicationType comunicationType)
    {
        try
        {
            _logger.LogInformation($"Sending SendEmail");

            if (string.IsNullOrWhiteSpace(reservation.Guest.Email))
                return false;

            string connectionString = _emailApiSettings.ConnectionString;
            var emailClient = new EmailClient(connectionString);
            var emailTemplate = await _templateService.GetEmailTemplate(property.PropertyId, comunicationType.ToString().ToLower());

            if (string.IsNullOrWhiteSpace(emailTemplate.SenderEmailAddress))
                return false;

            var fromAddress = emailTemplate.SenderEmailAddress;
            var toAddress = reservation.Guest.Email;
            string subject = GetEmailMessageSubject(emailTemplate, reservation.PmsReservationId);
            var body = $"{GetEmailMessageBody(emailTemplate, reservation, property, comunicationType)}";

            var emailSendOperation = await emailClient.SendAsync(
                Azure.WaitUntil.Completed,
                fromAddress,
                toAddress,
                subject,
                body);
            var statusMonitor = emailSendOperation.Value;
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in SendEmail");
            return false;
        }
    }
}
