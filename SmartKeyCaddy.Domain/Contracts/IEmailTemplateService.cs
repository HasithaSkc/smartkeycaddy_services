using SmartKeyCaddy.Models;

namespace SmartKeyCaddy.Domain.Contracts;
public interface IEmailTemplateService
{
    Task<EmailTemplate> GetEmailTemplate(int propertyId, string emailTemplateCode);
}
