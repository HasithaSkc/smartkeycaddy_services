﻿
using SmartKeyCaddy.Models;
using SmartKeyCaddy.Models.Messages;

namespace SmartKeyCaddy.Domain.Contracts;

public interface IKeyAllocationService
{
    Task<KeyAllocationResponse> CreateKeyAllocation(KeyAllocationRequest createKeyRequest);
    Task<List<KeyAllocation>> GetKeyAllocations(Guid deviceId);
    Task<KeyAllocation> GetKeyAllocation(Guid keyAllocationidId);
    Task<KeyAllocation> GetKeyAllocationHistory(string keyName);
    Task ProcessDeviceKeyTransaction(KeyTransactionMessage keyTransactionMessage);
    Task<bool> DeleteKey(Guid keyId);
    Task ProcessIndirectKeyAllocationMessages();
    Task ForceBinOpen(Guid deviceId, Guid binId);
}
