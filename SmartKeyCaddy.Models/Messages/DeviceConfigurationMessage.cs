namespace SmartKeyCaddy.Models.Messages;

public class DeviceConfigurationMessage : BaseMessage
{
    public List<Bin> Bins { get; set; }
    public List<KeyFobTag> KeyFobTags { get; set; }
    public List<DeviceSetting> DeviceSettings { get; set; }
}