using SmartKeyCaddy.Models;

namespace SmartKeyCaddy.Domain.Repository
{
    public interface IBinRepository
    {
        Task UpdateBinInUse(Guid binId, bool inUse);

        Task<List<Bin>> GetBins(Guid deviceId);
    }
}
