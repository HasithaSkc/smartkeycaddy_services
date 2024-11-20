using SmartKeyCaddy.Domain.Contracts;
using SmartKeyCaddy.Models;
using SmartKeyCaddy.Models.Exceptions;

namespace SmartKeyCaddy.Domain.Services;
public partial class PropertyRoomService
{
    private async Task ValidatePropertyRoomKeyFob(Guid keyFobTagId, Guid propertyId)
    { 
        var currentPropertyRoomkeyFobTag = await _propertyRoomRepository.GetPropertyRoomByKeyFobTag(keyFobTagId, propertyId);

        if (currentPropertyRoomkeyFobTag != null)
            throw new BadRequestException("Keyfobtag is already assigned to a room");
    }
}
