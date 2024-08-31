using SmartKeyCaddy.Models;

namespace SmartKeyCaddy.Domain.Repository;

public interface IKeyAllocationRepository
{
    Task<KeyAllocation> GetKeyAllocation(Guid deviceId, Guid keyId);
    Task<List<KeyAllocation>> GetKeyAllocations(Guid deviceId);
    Task<KeyAllocation> GetKeyAllocation(Guid keyAllocationId);
    Task InsertkeyAllocation(KeyAllocation keyAllocation);
    Task<KeyAllocation> GetKeyAllocationByKeyName(string keyName);
    Task UpdateKeyAllocation(KeyAllocation keyAllocation);
    Task<List<KeyAllocation>> GetUnsentKeyAllocations();
    Task UpdateKeyUnsentAllocationStatus(List<Guid> keyAllocationIds, Guid deviceId);
}
