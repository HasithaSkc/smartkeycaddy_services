namespace SmartKeyCaddy.Models;

public class PropertySetting
{
    public Guid PropertySettingId { get; set; }
    public Guid PropertyId { get; set; }
    public string SettingName { get; set; }
    public string SettingValue { get; set; }
}