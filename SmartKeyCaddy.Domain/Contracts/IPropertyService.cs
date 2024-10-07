using SmartKeyCaddy.Models;

namespace SmartKeyCaddy.Domain.Contracts;
public interface IPropertyService
{
    Task<Property> GetPropertyByCode(string propertyCode);
    Task<List<Property>> GetProperties();
    Task<Property> GetProperty(Guid propertyId);
    Task<Guid> AddProperty(Property property);
    Task<Guid> UpdateProperty(Property property);
    Task<Guid> DeleteProperty(Guid propertyId);
}
