

namespace SGAT.Application.Interfaces.Transactions
{
    //permite confirmar o revertir los cambios
    //permite liberar correctamente los recursos de la transacción de forma asíncrona
    public interface IAppTransaction : IAsyncDisposable
    {
        Task CommitAsync();

        Task RollbackAsync();
    }
}
