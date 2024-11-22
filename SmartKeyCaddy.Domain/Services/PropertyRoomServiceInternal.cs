using SmartKeyCaddy.Domain.Contracts;
using SmartKeyCaddy.Models;
using SmartKeyCaddy.Models.Exceptions;

namespace SmartKeyCaddy.Domain.Services;
public partial class PropertyRoomService
{
    private async Task ValidatePropertyRoomKeyFob(PropertyRoomKeyFobtag keyFobTag, Guid propertyId)
    { 
        var currentPropertyRoomkeyFobTag = await _propertyRoomRepository.GetPropertyRoomByKeyFobTag(keyFobTag.KeyFobTagId.Value, propertyId);

        if (currentPropertyRoomkeyFobTag == null)
            return;

        //Updating itself
        if (currentPropertyRoomkeyFobTag.PropertyRoomId == keyFobTag.PropertyRoomId)
            return;

        if (currentPropertyRoomkeyFobTag != null)
            throw new BadRequestException("Keyfobtag is already assigned to a room");
    }
}
