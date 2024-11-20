using SmartKeyCaddy.Models;

namespace SmartKeyCaddy.Domain.Repository
{
    public interface IBinRepository
    {
        Task UpdateBinInUse(Guid binId, bool inUse);
        Task<Bin> GetBin(Guid deviceId, Guid binId);
        Task<List<Bin>> GetBins(Guid deviceId, bool includeChildBins = true);
    }
}
