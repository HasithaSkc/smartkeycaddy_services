using Dapper;
using SmartKeyCaddy.Domain.Repository;
using SmartKeyCaddy.Models;

namespace SmartKeyCaddy.Repository;

public class EmailTemplateRepository : IEmailTemplateRepository
{
    private readonly IDBConnectionFactory _dbConnectionFactory;
    public EmailTemplateRepository(IDBConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<EmailTemplate> GetEmailTempalate(int propertyId, string empailTemplateCode)
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
}