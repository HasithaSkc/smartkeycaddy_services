﻿namespace SmartKeyCaddy.Common;

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
    KeyAlreadyExists
}

public enum MessageType
{
    KeyTransaction,
    DeviceRegistration,
    DeviceConfiguration,
    DirectKeyAllocation,
    IndirectKeyAllocation,
    Unknown
}

public enum CommunicationType
{
    OnlineChekin,
    OnlineChekinReminder,
    QrCode,
    KeyPinCode
}