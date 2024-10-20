using Dapper;
using SmartKeyCaddy.Common;
using SmartKeyCaddy.Domain.Repository;
using SmartKeyCaddy.Models;

namespace SmartKeyCaddy.Repository;

public class PropertyRepository : IPropertyRepository
{
    private readonly IDBConnectionFactory _dbConnectionFactory;
    public PropertyRepository(IDBConnectionFactory dbConnectionFactory) 
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    private string propertySql = $@"select property.propertyid
                            ,property.chainid
                            ,property.pmspropertyid
                            ,property.propertyname
                            ,property.propertycode
                            ,property.propertyshortcode
                            ,property.addressline1
                            ,property.city
                            ,property.statecode
                            ,property.postcode
                            ,country.countryid
                            ,country.countrycode
                            ,region.regionid
                            ,region.regionname
                            ,property.isactive
                            ,property.timezone
                            ,propertysetting.propertylogo
                            ,propertysetting.welcomemessage
                            ,propertysetting.backgroundimageurl
                            ,chain.chainid as chainchainid
                            ,chain.chainid
                            ,chain.chainname
                            ,chain.logourl
                            ,chain.chaincode
                            from {Constants.SmartKeyCaddySchemaName}.property 
                        inner join {Constants.SmartKeyCaddySchemaName}.chain on chain.chainid = property.chainid
                        inner join {Constants.SmartKeyCaddySchemaName}.country on country.countryid = property.countryid
                        inner join {Constants.SmartKeyCaddySchemaName}.region on region.regionid = property.regionid";

    private string _propertySettingSql = $@"select
                            propertysetting.propertyid
                            ,max(case WHEN settingname = 'propertylogo' then settingvalue end) as propertylogo
                            ,max(case WHEN settingname = 'welcomemessage' then settingvalue end) as welcomemessage
                            ,max(case WHEN settingname = 'backgroundimageurl' then settingvalue end) as backgroundimageurl
                        from {Constants.SmartKeyCaddySchemaName}.setting
                        inner join 
                            {Constants.SmartKeyCaddySchemaName}.propertysetting on propertysetting.settingid = setting.settingid
	                    inner join {Constants.SmartKeyCaddySchemaName}.property on property.propertyid = propertysetting.propertyid
                        group by
                            propertysetting.propertyid";


    public async Task<Property> GetProperty(Guid propertyId)
    {
        using (var connection = _dbConnectionFactory.CreateConnection())
        {
            var sql = @$"{propertySql} left join ({_propertySettingSql}) propertysetting on propertysetting.propertyid = property.propertyid
                                where property.propertyid = @propertyId";

            return (await connection.QueryAsync<Property, Chain, Property>(sql, (property, chain) => {
                property.Chain = chain;
                return property;
            },
                new
                {
                    propertyId
                },
            splitOn: "chainchainid")).FirstOrDefault();
        }
    }

    public async Task<Property> GetPropertyByCode(string propertyCode)
    {
        using (var connection = _dbConnectionFactory.CreateConnection())
        {
            var sql = @$"{propertySql} inner join ({_propertySettingSql}) propertysetting on propertysetting.propertyid = property.propertyid
                            where lower(propertyCode) = lower(@propertyCode)";

            return (await connection.QueryAsync<Property, Chain, Property>(sql, (property, chain) => {
                property.Chain = chain;
                return property;
            },
            new
            {
                propertyCode
            },
             splitOn: "ChainChainId")).SingleOrDefault();
        }
    }

    public async Task<List<Property>> GetPropertyList()
    {
        using (var connection = _dbConnectionFactory.CreateConnection())
        {
            var sql = $"{propertySql} inner join ({_propertySettingSql}) propertysetting on propertysetting.propertyid = property.propertyid";

            return (await connection.QueryAsync<Property, Chain, Property>(sql, (property, chain) => {
                property.Chain = chain;
                return property;
            },
            splitOn: "ChainChainId")).ToList();
        }
    }
}