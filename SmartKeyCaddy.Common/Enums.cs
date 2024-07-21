namespace SmartKeyCaddy.Common;

public static class DeviceResponseStatus
{
    public static int Success = 200;
}

public enum KeyAllocationStatus
{
    KeyCreated,
    KeyAllocated,
    KeyDroppedOff,
    KeyPickedUp,
    KeyExistsOnServer,
    KeyExistsOnDevice
}

public enum DeviceMessageType
{
    KeyTransaction,
    Unknown
}
