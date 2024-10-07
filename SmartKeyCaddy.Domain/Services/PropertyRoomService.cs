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

    public async Task InsertPropertyRoomsKeyFobTags(List<PropertyRoomKeyFobtag> propertyRoomKeyFobTags, Guid propertyId)
    {
        if (!propertyRoomKeyFobTags.Any()) return;

        var proeprtyRoomIds = propertyRoomKeyFobTags.Select(room=>room.PropertyRoomId).ToList();
        await _propertyRoomRepository.DeletePropertyRoomKeyFobTags(propertyId);
        var property = await _propertyRepository.GetProperty(propertyId);

        foreach (var propertyRoomKeyFobTag in propertyRoomKeyFobTags)
        {
            await _propertyRoomRepository.InsertPropertyRoomKeyFobTag(propertyRoomKeyFobTag, property.PropertyId, property.Chain.ChainId);
        }
    }
}
