using Dapper;
using SmartKeyCaddy.Domain.Repository;
using SmartKeyCaddy.Models;

namespace SmartKeyCaddy.Repository;

public class LocationRepository : ILocationRepository
{
    private readonly IDBConnectionFactory _dbConnectionFactory;
    public LocationRepository(IDBConnectionFactory dbConnectionFactory) 
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<List<Location>> GetLocations()
    {
        using var connection = _dbConnectionFactory.CreateConnection();

        var sql = @$"select regionid, regionname, chainid, countryid, parentregionid, createddatetime, lastupdateddatetime from smartkeycaddyuser.region
                    order by regionname";

        return (await connection.QueryAsync<Location>(sql)).ToList();
    }

    public async Task<Location> GetLocation(Guid lcoationId)
    {
        using var connection = _dbConnectionFactory.CreateConnection();

        var sql = @$"select regionid, regionname, chainid, countryid, parentregionid, createddatetime, lastupdateddatetime from smartkeycaddyuser.region
                    order by regionname";

        return (await connection.QueryAsync<Location>(sql)).SingleOrDefault();
    }

    public async Task AddLocation(Location location)
    {
        using var connection = _dbConnectionFactory.CreateConnection();

        var sql = @$"insert into smartkeycaddyuser.region(regionname, chainid, countryid, parentregionid, createddatetime, lastupdateddatetime)
	                 values (?, ?, ?, ?, ?, ?, ?)";
        await connection.QueryAsync<Location>(sql,
            new
            {
                location.LocationName,
                location.ChainId,
                location.CountryId,
                location.ParentRegionId,
                CreatedDateTime = DateTime.UtcNow,
            });

    }

    public Task AddLocation(Guid parentId, Location location)
    {
        throw new NotImplementedException();
    }

    public Task<Guid> UpdateLocation(Location location)
    {
        throw new NotImplementedException();
    }

    public Task DeleteLocation(Guid lcoationId)
    {
        throw new NotImplementedException();
    }

    Task<Guid> ILocationRepository.AddLocation(Location location)
    {
        throw new NotImplementedException();
    }
}