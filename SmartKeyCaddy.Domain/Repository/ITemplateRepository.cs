using SmartKeyCaddy.Models;

namespace SmartKeyCaddy.Domain.Repository;
public interface ITemplateRepository
{
    Task<EmailTemplate> GetEmailTempalate(Guid propertyId, string empailTemplateCode);
    Task<SmsTemplate> GetSmsTempalate(Guid properetyId, string smsTemplateCode);
}
