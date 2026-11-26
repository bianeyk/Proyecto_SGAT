using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace SGAT.Application.Services;

internal static class DatabaseErrorHelper
{
    public static bool IsUniqueConstraintViolation(
        DbUpdateException exception)
    {
        for (var inner = exception.InnerException;
             inner is not null;
             inner = inner.InnerException)
        {
            if (inner is SqlException sqlException
                && sqlException.Errors.Cast<SqlError>()
                    .Any(error => error.Number is 2601 or 2627))
            {
                return true;
            }
        }

        return false;
    }
}