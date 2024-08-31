namespace HotelCheckIn.Domain.Contracts;

public interface IPropertySettingService
{
    Task<T> GetPropertySetting<T>(string settingName, int propertyId, T defaultValue);
}
