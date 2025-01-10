using Microsoft.Extensions.Logging;
using SmartKeyCaddy.Domain.Contracts;
using SmartKeyCaddy.Domain.Repository;
using SmartKeyCaddy.Models;

namespace SmartKeyCaddy.Domain.Services;
public partial class PropertyRoomService : IPropertyRoomService
{
    private readonly ILogger<PropertyRoomService> _logger;
    private readonly IPropertyRoomRepository _propertyRoomRepository;
    private readonly IPropertyRepository _propertyRepository;

    public PropertyRoomService(ILogger<PropertyRoomService> logger,
        IPropertyRoomRepository propertyRoomRepository,
        IPropertyRepository propertyRepository)
    {
        _logger = logger;
        _propertyRoomRepository = propertyRoomRepository;
        _propertyRepository = propertyRepository;
    }

    public async Task<List<PropertyRoom>> GetPropertyRooms(Guid propertyId)
    {
        return await _propertyRoomRepository.GetPropertyRooms(propertyId);
    }

    public async Task<List<PropertyRoom>> GetPropertyRoomsKeyFobTags(Guid propertyId)
    {
        return await _propertyRoomRepository.GetPropertyRoomKeyFobTags(propertyId);
    }

    public async Task InsertOrUpdatePropertyRoomsKeyFobTag(PropertyRoomKeyFobtag propertyRoomkeyFobTag, Guid propertyId)
    {
        if (propertyRoomkeyFobTag == null) return;

        if (propertyRoomkeyFobTag.KeyFobTagId == Guid.Empty || propertyRoomkeyFobTag.KeyFobTagId == null)
        {
            await _propertyRoomRepository.DeletePropertyRoomKeyFobTag(propertyId, propertyRoomkeyFobTag.PropertyRoomId);
            return;
        }

        await ValidatePropertyRoomKeyFob(propertyRoomkeyFobTag, propertyId);

        var currentPropertyRoomkeyFobTag = await _propertyRoomRepository.GetPropertyRoomKeyFobTag(propertyId, propertyRoomkeyFobTag.PropertyRoomId);

        if (currentPropertyRoomkeyFobTag != null)
        { 
            await _propertyRoomRepository.UpdatePropertyRoomKeyFobTag(propertyRoomkeyFobTag, propertyId);
            return;
        }

        var property = await _propertyRepository.GetProperty(propertyId);
        await _propertyRoomRepository.InsertPropertyRoomKeyFobTag(propertyRoomkeyFobTag, propertyId, property.ChainId);
    }
}
