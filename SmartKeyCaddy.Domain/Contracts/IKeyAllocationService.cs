
using SmartKeyCaddy.Models;
using SmartKeyCaddy.Models.Messages;

namespace SmartKeyCaddy.Domain.Contracts;

public interface IKeyAllocationService
{
    Task<KeyAllocationResponse> CreateKeyAllocation(KeyAllocationRequest createKeyRequest);
    Task<List<KeyAllocation>> GetKeyAllocations(Guid deviceId);
    Task ProcessDeviceKeyTransaction(KeyTransactionMessage keyTransactionMessage);
    Task<bool> DeleteKey(Guid keyId);
}
