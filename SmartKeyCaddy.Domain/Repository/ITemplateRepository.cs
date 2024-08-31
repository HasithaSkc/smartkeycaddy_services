using SmartKeyCaddy.Models;

namespace HotelCheckIn.Domain.Contracts;
public interface ITemplateRepository
{
    Task<EmailTemplate> GetEmailTempalate(Guid propertyId, string empailTemplateCode);
    Task<SmsTemplate> GetSmsTempalate(Guid properetyId, string smsTemplateCode);
}
