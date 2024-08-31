namespace SmartKeyCaddy.Models;
public class Property 
{
    public Guid PropertyId { get; set; }
    public string PmsPropertyId { get; set; }
    public string PropertyName { get; set; }
    public string PropertyCode { get; set; }
    public string Address { get; set; }
    public string Abn { get; set; }
    public bool IsActive { get; set; }
    public string FrontDeskEmail { get; set; }
    public string HomeBackgroundImageUrl { get; set; }
    public string BackgroundImageUrl { get; set; }
    public string MessageOnReceipt { get; set; }
    public string WelcomeMessage { get; set; }
    public string QrCode { get; set; }
    public string PhoneNumber { get; set; }
    public bool EnableSupportEmails { get; set; }
    public Chain Chain { get; set; }
    public string PinAllocationMethod { get; set; }
    public string KeyCafeStartTime { get; set; }
    public string KeyCafeEndTime { get; set; }
    public bool AllowCheckInIfRoomNotReady { get; set; }
    public bool IsIdScanEnabled { get; set; }
    public bool IsQrCodeEnabled { get; set; }
    public bool IsOnlineCheckinEmailEnabled { get; set; }
    public int OnlineCheckinEmailDueDays { get; set; }
    public int DataSyncPeriod { get; set; }
    public string PropertyShortCode { get; set; }
    public string TimeZone { get; set; }
    public string PropertyLogo { get; set; }
    public bool UpdateAddonAmount { get; set; }
    public bool IsQrCodeAutomated { get; set; }
    public bool UpdateRoomNameFromGuestDescription { get; set; }
    public string CroNumber { get; set; }
    public string KeyProvider { get; set; }
}
