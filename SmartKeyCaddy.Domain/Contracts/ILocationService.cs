using SmartKeyCaddy.Models;

namespace SmartKeyCaddy.Domain.Contracts;
public interface ILocationService
{
    Task<List<Location>> GetLocations(Guid chainId);
    Task<Location> GetLocation(Guid locationId);
    Task AddLocation(Location location, Guid chainId);
    Task AddLocation(Guid parentId, Guid chainId);
    Task UpdateLocation(Location location, Guid chainId);
    Task DeleteLocation(Guid locationId);
}
