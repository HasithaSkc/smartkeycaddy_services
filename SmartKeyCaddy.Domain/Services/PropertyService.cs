using Microsoft.Extensions.Logging;
using SmartKeyCaddy.Domain.Contracts;
using SmartKeyCaddy.Domain.Repository;
using SmartKeyCaddy.Models;

namespace SmartKeyCaddy.Domain.Services;
public partial class PropertyService : IPropertyService
{
    private readonly ILogger<PropertyService> _logger;
    private readonly IPropertyRepository _propertyRepository;

    public PropertyService(ILogger<PropertyService> logger,
        IPropertyRepository propertyRepository)
    {
        _logger = logger;
        _propertyRepository = propertyRepository;
    }

    public async Task<Property> GetPropertyByCode(string propertyCode)
    {
        return await _propertyRepository.GetPropertyByCode(propertyCode);
    }

    public async Task<Property> GetProperty(Guid propertyUuid)
    {
        return await _propertyRepository.GetProperty(propertyUuid);
    }

    public async Task<List<Property>> GetProperties()
    {
        return await _propertyRepository.GetPropertyList();
    }

    public Task<Guid> AddProperty(Property property)
    {
        throw new NotImplementedException();
    }

    public Task<Guid> UpdateProperty(Property property)
    {
        throw new NotImplementedException();
    }

    public Task<Guid> DeleteProperty(Guid propertyId)
    {
        throw new NotImplementedException();
    }
}
