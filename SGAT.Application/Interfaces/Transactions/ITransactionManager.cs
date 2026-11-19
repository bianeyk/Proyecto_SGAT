using System.Data;

//inicia una transacción.
namespace SGAT.Application.Interfaces.Transactions
{
    public interface ITransactionManager
    {
        Task<IAppTransaction> BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
    }
}
