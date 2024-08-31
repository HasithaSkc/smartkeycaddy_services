using SmartKeyCaddy.Models;

namespace SmartKeyCaddy.Domain.Repository;
public interface IPropertyRepository
{
    Task<Property> GetPropertyByPmsPropertyId(string pmsPropertyId);
    Task<Property> GetPropertyByCode(string propertyCode);
    Task<List<Property>> GetPropertyList();
    Task<Property> GetPropertyById(Guid propertyId);
}
