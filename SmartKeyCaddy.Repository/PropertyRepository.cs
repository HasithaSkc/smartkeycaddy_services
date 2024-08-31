using Dapper;
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

    private string propertySql = @"select property.propertyid
                            ,propertyuuid
                            ,pmspropertyid
                            ,propertyname
                            ,propertycode
                            ,propertyshortcode
                            ,address
                            ,abn
                            ,property.isactive
                            ,timezone
                            ,propertysetting.datasyncperiod
                            ,propertysetting.isonlinecheckinemailenabled
                            ,propertysetting.isqrcodeenabled
                            ,propertysetting.updateaddonamount
                            ,propertysetting.onlinecheckinemailduedays
                            ,propertysetting.propertylogo
                            ,propertysetting.frontdeskemail
                            ,propertysetting.homebackgroundimageurl
                            ,propertysetting.enablesupportemails
                            ,propertysetting.qrcode
                            ,propertysetting.messageonreceipt
                            ,propertysetting.welcomemessage
                            ,propertysetting.backgroundimageurl
                            ,propertysetting.pinallocationmethod
                            ,propertysetting.allowcheckinifroomnotready
                            ,propertysetting.keycafestarttime
                            ,propertysetting.keycafeendtime
                            ,propertysetting.isidscanenabled
                            ,propertysetting.isqrcodeautomated
                            ,propertysetting.iscommunicationenabled
                            ,propertysetting.updateroomnamefromguestdescription
                            ,propertysetting.cronumber
                            ,propertysetting.keyprovider
                            ,chain.chainid as chainchainid
                            ,chain.chainid
                            ,chain.chainuuid
                            ,chain.chainname
                            ,chain.logourl
                            ,coalesce(propertysetting.cronumber, chain.cronumber) cronumber   
                            ,chain.chaincode
                            from property inner join chain on chain.chainid = property.chainid";

    private string _propertySettingSql = @"select
                            propertyid
                            ,max(case when settingname = 'datasyncperiod' then settingvalue end) as datasyncperiod
                            ,max(case WHEN settingname = 'isonlinecheckinemailenabled' then settingvalue end) as isonlinecheckinemailenabled
                            ,max(case WHEN settingname = 'isqrcodeenabled' then settingvalue end) as isqrcodeenabled
                            ,max(case WHEN settingname = 'updateaddonamount' then settingvalue end) as updateaddonamount
                            ,max(case WHEN settingname = 'onlinecheckinemailduedays' then settingvalue end) as onlinecheckinemailduedays
                            ,max(case WHEN settingname = 'propertylogo' then settingvalue end) as propertylogo
                            ,max(case WHEN settingname = 'frontdeskemail' then settingvalue end) as frontdeskemail
                            ,max(case WHEN settingname = 'homebackgroundimageurl' then settingvalue end) as homebackgroundimageurl
                            ,max(case WHEN settingname = 'enablesupportemails' then settingvalue end) as enablesupportemails
                            ,max(case WHEN settingname = 'qrcode' then settingvalue end) as qrcode
                            ,max(case WHEN settingname = 'messageonreceipt' then settingvalue end) as messageonreceipt
                            ,max(case WHEN settingname = 'welcomemessage' then settingvalue end) as welcomemessage
                            ,max(case WHEN settingname = 'backgroundimageurl' then settingvalue end) as backgroundimageurl
                            ,max(case WHEN settingname = 'pinallocationmethod' then settingvalue end) as pinallocationmethod
                            ,max(case WHEN settingname = 'allowcheckinifroomnotready' then settingvalue end) as allowcheckinifroomnotready
                            ,max(case WHEN settingname = 'keycafestarttime' then settingvalue end) as keycafestarttime
                            ,max(case WHEN settingname = 'keycafeendtime' then settingvalue end) as keycafeendtime
                            ,max(case WHEN settingname = 'isidscanenabled' then settingvalue end) as isidscanenabled
                            ,max(case WHEN settingname = 'iscommunicationenabled' then settingvalue end) as iscommunicationenabled
                            ,max(case WHEN settingname = 'isqrcodeautomated' then settingvalue end) as isqrcodeautomated
                            ,max(case WHEN settingname = 'updateroomnamefromguestdescription' then settingvalue end) as updateroomnamefromguestdescription
                            ,max(case WHEN settingname = 'cronumber' then settingvalue end) as cronumber
                            ,max(case WHEN settingname = 'keyprovider' then settingvalue end) as keyprovider
                        from propertysetting
                        group by propertyid";

    public async Task<Property> GetPropertyByPmsPropertyId(string pmsPropertyId)
    {
        using (var connection = _dbConnectionFactory.CreateConnection())
        {
            var sql = @$"{propertySql} inner join ({_propertySettingSql}) propertysetting on propertysetting.propertyid = property.propertyid
                                where pmspropertyid = @pmsPropertyId";

            return (await connection.QueryAsync<Property, Chain, Property>(sql, (property, chain) => {
                property.Chain = chain;
                return property;
            },
                new
                {
                    pmsPropertyId
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

    public async Task<Property> GetPropertyById(Guid propertyId)
    {
        using (var connection = _dbConnectionFactory.CreateConnection())
        {
            var sql = @$"{propertySql} inner join ({_propertySettingSql}) propertysetting on propertysetting.propertyid = property.propertyid
                            where propertyuuid = @propertyId";

            return (await connection.QueryAsync<Property, Chain, Property>(sql, (property, chain) => {
                property.Chain = chain;
                return property;
            },
                new
                {
                    propertyId
                },
            splitOn: "ChainChainId")).FirstOrDefault();
        }
    }
}