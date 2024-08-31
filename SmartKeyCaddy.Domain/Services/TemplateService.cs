using Microsoft.Extensions.Logging;
using HotelCheckIn.Domain.Contracts;
using SmartKeyCaddy.Domain.Contracts;
using SmartKeyCaddy.Models;

namespace SmartKeyCaddy.Domain.Services;

public class TemplateService : ITemplateService
{
    private readonly ITemplateRepository _templateRepository;
    private readonly ILogger<EmailService> _logger;

    public TemplateService(ILogger<EmailService> logger,
        ITemplateRepository templateRepository)
    {
        _logger = logger;
        _templateRepository = templateRepository;
    }

    public async Task<EmailTemplate> GetEmailTemplate(Guid propertyId, string emailTemplateCode)
    {
        return await _templateRepository.GetEmailTempalate(propertyId, emailTemplateCode);
    }

    public async Task<SmsTemplate> GetSmsTemplate(Guid propertyId, string smsTemplateCode)
    {
        return await _templateRepository.GetSmsTempalate(propertyId, smsTemplateCode);
    }
}
