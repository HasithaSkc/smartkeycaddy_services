using SmartKeyCaddy.Models;

namespace SmartKeyCaddy.Domain.Contracts;

public interface ITemplateService
{
    Task<EmailTemplate> GetEmailTemplate(Guid propertyId, string emailTemplateCode);
    Task<SmsTemplate> GetSmsTemplate(Guid propertyId, string smsTemplateCode);
}
