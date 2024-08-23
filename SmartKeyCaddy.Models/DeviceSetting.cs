namespace SmartKeyCaddy.Models;

public class DeviceSetting
{
    public Guid DeviceSettingId { get; set; }
    public string SettingName { get; set; }
    public string SettingValue { get; set; }
    public Guid DeviceId { get; set; }
}