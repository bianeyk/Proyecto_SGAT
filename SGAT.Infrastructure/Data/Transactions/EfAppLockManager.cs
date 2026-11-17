
using Microsoft.EntityFrameworkCore;
using SGAT.Application.Interfaces.Transactions;
using SGAT.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace SGAT.Infrastructure.Data.Transactions
{
    public class EfAppLockManager : IAppLockManager
    {
        private readonly ApplicationDbContext _context;

        public EfAppLockManager(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AcquireLockAsync(string resource)
        {
            //valida que el nombre del recurso no sea nulo o vacío
            if (string.IsNullOrWhiteSpace(resource))
            {
                throw new ArgumentException(
                    "El nombre del recurso de bloqueo es obligatorio.",
                    nameof(resource));
            }
            //obtiene la conexión que utiliza EF Core.
            var connection = _context.Database.GetDbConnection();
            //prepara una instrucción SQL
            await using var command = connection.CreateCommand();

            var currentTransaction = _context.Database.CurrentTransaction;

            if (currentTransaction == null)
            {
                throw new InvalidOperationException(
                    "No existe una transacción activa para adquirir el bloqueo.");
            }
            command.CommandText = @"
                DECLARE @result INT;

                EXEC @result = sp_getapplock
                    @Resource = @resource,
                    @LockMode = 'Exclusive',
                    @LockOwner = 'Transaction',
                    @LockTimeout = 10000;

                SELECT @result;
            ";

            var parameter = command.CreateParameter();
            parameter.ParameterName = "@resource";
            parameter.Value = resource;

            command.Parameters.Add(parameter);
            //obtiene la transacción que ya iniciamos con EfTransactionManager
            command.Transaction = currentTransaction.GetDbTransaction();

            var result = await command.ExecuteScalarAsync();

            var lockResult = Convert.ToInt32(result);

            if (lockResult < 0)
            {
                throw new InvalidOperationException(
                    $"No se pudo obtener el bloqueo '{resource}'. Código: {lockResult}.");
            }
        }
    }
}
