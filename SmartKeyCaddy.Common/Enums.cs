namespace SmartKeyCaddy.Common;

public static class DeviceResponseStatus
{
    public static int Success = 200;
}

public enum KeyAllocationStatus
{
    KeyCreated,
    KeyCreatedInServer,
    KeyLoaded,
    KeyPickedUp,
    KeyDroppedOff,
    BinForceOpened
}

public enum KeyAllocationErrorStatus
{
    KeyAlreadyExists,
    KeyAlreadyLoaded,
    KeyFobNotFound
}

public enum MessageType
{
    KeyTransaction,
    DeviceRegistration,
    DeviceConfiguration,
    DirectKeyAllocation,
    IndirectKeyAllocation,
    ForceBinOpen,
    Unknown
}

public enum CommunicationType
{
    OnlineChekin,
    OnlineChekinReminder,
    QrCode,
    KeyPinCode
}

public enum KeyAllocationType
{
    Managed,
    SelfManaged,
    Isolated,
}