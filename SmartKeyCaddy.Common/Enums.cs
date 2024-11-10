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
}

public enum KeyAllocationErrorStatus
{
    KeyAlreadyExists,
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

public enum KeyTransactionType
{
    ForceBinOpen
}