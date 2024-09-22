using Microsoft.Extensions.Logging;
using SmartKeyCaddy.Domain.Contracts;
using SmartKeyCaddy.Domain.Repository;
using SmartKeyCaddy.Models;

namespace SmartKeyCaddy.Domain.Services;
public partial class PropertyRoomService : IPropertyRoomService
{
    private readonly ILogger<PropertyRoomService> _logger;
    private readonly IPropertyRoomRepository _propertyRoomRepository;

    public PropertyRoomService(ILogger<PropertyRoomService> logger,
        IPropertyRoomRepository propertyRoomRepository)
    {
        _logger = logger;
        _propertyRoomRepository = propertyRoomRepository;
    }

    public async Task<List<PropertyRoom>> GetPropertyRooms(Guid propertyId)
    {
        return await _propertyRoomRepository.GetPropertyRooms(propertyId);
    }
}
