using Microsoft.Extensions.Logging;
using SmartKeyCaddy.Domain.Contracts;
using SmartKeyCaddy.Domain.Repository;
using SmartKeyCaddy.Models;

namespace SmartKeyCaddy.Domain.Services;
public partial class LocationService : ILocationService
{
    private readonly ILogger<ILocationService> _logger;
    private readonly ILocationRepository _locationRepository;

    public LocationService(ILogger<ILocationService> logger,
        ILocationRepository locationRepository)
    {
        _logger = logger;
        _locationRepository = locationRepository;
    }

    public async Task<List<Location>> GetLocations(Guid chainId)
    {
        return await _locationRepository.GetLocations();
    }

    public async Task<Location> GetLocation(Guid locationId)
    {
        return await _locationRepository.GetLocation(locationId);
    }

    public async Task AddLocation(Location location)
    {
        await _locationRepository.AddLocation(location);
    }

    public async Task AddLocation( Guid parentId, Location location)
    {
        await _locationRepository.AddLocation(parentId, location);
    }

    public async Task UpdateLocation(Location location)
    {
        await _locationRepository.UpdateLocation(location);
    }

    public async Task DeleteLocation(Guid lcoationId)
    {
        await _locationRepository.DeleteLocation(lcoationId);
    }

    public Task AddLocation(Location location, Guid chainId)
    {
        throw new NotImplementedException();
    }

    public Task AddLocation(Guid parentId, Guid chainId)
    {
        throw new NotImplementedException();
    }

    public Task UpdateLocation(Location location, Guid chainId)
    {
        throw new NotImplementedException();
    }
}
