
using Microsoft.EntityFrameworkCore;
using SGAT.Application.Interfaces.Transactions;
using SGAT.Infrastructure.Data;
using System.Data;

namespace SGAT.Infrastructure.Data.Transactions
{
    //Esta clase será responsable de iniciar transacciones utilizando Entity Framework Core
    public class EfTransactionManager : ITransactionManager
    {
        private readonly ApplicationDbContext _context;

        public EfTransactionManager(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IAppTransaction> BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            var transaction = await _context.Database
                .BeginTransactionAsync(isolationLevel);

            return new EfAppTransaction(transaction);
        }
    }
}
