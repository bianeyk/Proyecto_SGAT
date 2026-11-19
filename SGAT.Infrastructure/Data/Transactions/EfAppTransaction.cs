
using Microsoft.EntityFrameworkCore.Storage;
using SGAT.Application.Interfaces.Transactions;

namespace SGAT.Infrastructure.Data.Transactions
{
    //Esta clase implementará las operaciones de confirmar, revertir y liberar la transacción
    public class EfAppTransaction : IAppTransaction
    {
        private readonly IDbContextTransaction _transaction;

        public EfAppTransaction(IDbContextTransaction transaction)
        {
            _transaction = transaction;
        }

        public Task CommitAsync()
        {
            return _transaction.CommitAsync();
        }

        public Task RollbackAsync()
        {
            return _transaction.RollbackAsync();
        }

        public ValueTask DisposeAsync()
        {
            return _transaction.DisposeAsync();
        }
    }
}
