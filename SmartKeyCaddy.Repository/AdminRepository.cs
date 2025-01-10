using SmartKeyCaddy.Domain.Repository;

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