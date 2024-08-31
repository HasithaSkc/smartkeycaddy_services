using SmartKeyCaddy.Models;

namespace SmartKeyCaddy.Domain.Contracts;
public interface IPropertyService
{
    Task<Property> GetPropertyByCode(string propertyCode);
    Task<List<Property>> GetPropertyList();
    Task<Property> GetPropertyById(Guid propertyUuid);
}
