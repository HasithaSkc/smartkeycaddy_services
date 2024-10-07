using SmartKeyCaddy.Models;

namespace SmartKeyCaddy.Domain.Repository;
public interface ILocationRepository
{
    Task<List<Location>> GetLocations();
    Task<Location> GetLocation(Guid lcoationId);
    Task<Guid> AddLocation(Location location);
    Task AddLocation(Guid parentId, Location location);
    Task<Guid> UpdateLocation(Location location);
    Task DeleteLocation(Guid lcoationId);
}
