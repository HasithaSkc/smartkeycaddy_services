using System.Transactions;

namespace SmartKeyCaddy.Domain.DomainHelper
{
    public class TransactionHelper
    {
        public static TransactionScope CreateTransactionScope()
        {
            var transactionOptions = new TransactionOptions()
            {
                IsolationLevel = IsolationLevel.ReadCommitted
            };

            return new TransactionScope(TransactionScopeOption.Required, transactionOptions, TransactionScopeAsyncFlowOption.Enabled);
        }
    }
}
