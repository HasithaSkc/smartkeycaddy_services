using SmartKeyCaddy.Models;

namespace SmartKeyCaddy.Domain.Repository;

public interface IKeyTransactionReposiotry
{
    Task InsertKeyTransaction(KeyTransaction keyTransaction);
}
