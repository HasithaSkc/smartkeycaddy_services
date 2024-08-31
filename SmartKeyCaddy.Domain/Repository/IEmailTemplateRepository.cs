
using SmartKeyCaddy.Models;

namespace SmartKeyCaddy.Domain.Repository;

public interface IEmailTemplateRepository
{
    Task<EmailTemplate> GetEmailTempalate(int propertyId, string empailTemplateCode);
}
