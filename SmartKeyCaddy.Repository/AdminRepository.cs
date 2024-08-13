using Dapper;
using SmartKeyCaddy.Common;
using SmartKeyCaddy.Domain.Repository;
using SmartKeyCaddy.Models;

namespace SmartKeyCaddy.Repository
{
    public class AdminRepository : IAdminRepository
    {
        private readonly IDBConnectionFactory _dbConnectionFactory;
        public AdminRepository(IDBConnectionFactory dbConnectionFactory) 
        {
            _dbConnectionFactory = dbConnectionFactory;
        }
    }
}