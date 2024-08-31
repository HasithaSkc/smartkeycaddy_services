namespace SmartKeyCaddy.Common;

public static class DeviceResponseStatus
{
    public static int Success = 200;
}

public enum KeyAllocationStatus
{
    KeyCreated,
    KeyLoaded,
    KeyPickedUp,
    KeyDroppedOff,
    KeyAlreadyExists
}

public enum MessageType
{
    KeyTransaction,
    DeviceRegistration,
    DeviceConfiguration,
    Unknown
}

public enum CommunicationType
{
    OnlineChekin,
    OnlineChekinReminder,
    QrCode,
    KeyPinCode
}