
namespace SGAT.Application.Interfaces.Transactions
{
    public interface IAppLockManager
    {
        Task AcquireLockAsync(string resource);
    }
}
