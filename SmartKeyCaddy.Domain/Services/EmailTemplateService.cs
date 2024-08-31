using Microsoft.Extensions.Logging;
using SmartKeyCaddy.Domain.Contracts;
using SmartKeyCaddy.Domain.Repository;
using SmartKeyCaddy.Models;

namespace SmartKeyCaddy.Domain.Services;

public class EmailTemplateService : IEmailTemplateService
{
    private readonly IEmailTemplateRepository _templateRepository;
    private readonly ILogger<EmailService> _logger;

    public EmailTemplateService(ILogger<EmailService> logger,
        IEmailTemplateRepository templateRepository)
    {
        _logger = logger;
        _templateRepository = templateRepository;
    }

    public async Task<EmailTemplate> GetEmailTemplate(int propertyId, string emailTemplateCode)
    {
        return await _templateRepository.GetEmailTempalate(propertyId, emailTemplateCode);
    }
}
