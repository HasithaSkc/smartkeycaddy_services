using Microsoft.Azure.Devices;
using SmartKeyCaddy.Models;

namespace SmartKeyCaddy.Domain.Repository
{
    public interface IKeyAllocationRepository
    {
        Task<KeyAllocation> GetKeyAllocation(Guid deviceId, Guid keyId);
        Task<List<KeyAllocation>> GetKeyAllocations(Guid deviceId);
        Task<KeyAllocation> GetKeyAllocation(Guid keyAllocationId);
        Task Insertkey(KeyAllocation keyAllocation);
        Task<bool> KeyExists(string keyName, DateTime? checkinDate);
        Task UpdateKeyAllocation(KeyAllocation keyAllocation);
    }
}
