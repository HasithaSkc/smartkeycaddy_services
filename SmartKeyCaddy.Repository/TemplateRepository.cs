using Dapper;
using SmartKeyCaddy.Domain.Repository;
using SmartKeyCaddy.Models;

namespace SmartKeyCaddy.Repository;

public class TemplateRepository : ITemplateRepository
{
    private readonly IDBConnectionFactory _dbConnectionFactory;
    public TemplateRepository(IDBConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<EmailTemplate> GetEmailTempalate(Guid propertyId, string empailTemplateCode)
    {
        using (var connection = _dbConnectionFactory.CreateConnection())
        {
            var sql = @"select emailtemplatecode
                                ,emailtemplateuuid
                                ,emailtemplatedescription
                                ,emailsubject
                                ,emailbody
                                ,senderemailaddress
                                ,emailsignatureimagepath
                                ,propertyid
                                ,emailtemplate.isactive
                            from emailtemplate inner join propertyemailtemplate on propertyemailtemplate.emailtemplateid = emailtemplate.emailtemplateid
                            where propertyemailtemplate.propertyid = @propertyId and emailtemplatecode = @empailTemplateCode and emailtemplate.isactive = true";
            return (await connection.QueryAsync<EmailTemplate>(sql,
        new
        {
            empailTemplateCode,
            propertyId
        })).FirstOrDefault();
        }
    }

    public async Task<SmsTemplate> GetSmsTempalate(Guid properetyId, string smsTemplateCode)
    {
        using (var connection = _dbConnectionFactory.CreateConnection())
        {
            var sql = @"select smstemplatecode
                                ,smstemplateuuid
                                ,smstemplatedescription
                                ,smsbody
                                ,alphatag as sendername
                                ,propertyid
                                ,smstemplate.isactive
                            from smstemplate inner join propertysmstemplate on propertysmstemplate.smstemplateid = smstemplate.smstemplateid
                            where propertysmstemplate.propertyid = @propertyId and smstemplatecode = @smsTemplateCode and smstemplate.isactive = true";
            return (await connection.QueryAsync<SmsTemplate>(sql,
        new
        {
            smsTemplateCode,
            properetyId
        })).FirstOrDefault();
        }
    }
}