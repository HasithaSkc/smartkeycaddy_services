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

    public async Task<Property> GetPropertyById(Guid propertyUuid)
    {
        return await _propertyRepository.GetPropertyById(propertyUuid);
    }

    public async Task<List<Property>> GetPropertyList()
    {
        return await _propertyRepository.GetPropertyList();
    }
}
