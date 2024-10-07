using SmartKeyCaddy.Models;

namespace SmartKeyCaddy.Domain.Repository;
public interface IPropertyRepository
{
    Task<Property> GetProperty(Guid propertyId);
    Task<Property> GetPropertyByCode(string propertyCode);
    Task<List<Property>> GetPropertyList();
}
