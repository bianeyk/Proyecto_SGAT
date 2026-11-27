using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Net.Http.Headers;
using System.Globalization;
using System.Threading.RateLimiting;

namespace SGAT.Web.Security
{
    public static class RateLimitRejectionHandler
    {
        public static async ValueTask HandleAsync(
            OnRejectedContext context,
            CancellationToken cancellationToken)
        {
            var httpContext = context.HttpContext;
            var response = httpContext.Response;

            response.StatusCode =
                StatusCodes.Status429TooManyRequests;

            response.ContentType =
                "text/plain; charset=utf-8";

            response.Headers[HeaderNames.CacheControl] =
                "no-store";

            var mensaje =
                "Demasiadas solicitudes. Intenta nuevamente más tarde.";

            // Obtener el tiempo estimado de espera.
            if (context.Lease.TryGetMetadata(
                MetadataName.RetryAfter,
                out var retryAfter))
            {
                var segundos = Math.Max(
                    1,
                    (int)Math.Ceiling(retryAfter.TotalSeconds));

                response.Headers[HeaderNames.RetryAfter] =
                    segundos.ToString(CultureInfo.InvariantCulture);

                mensaje =
                    $"Demasiadas solicitudes. " +
                    $"Espera aproximadamente {segundos} segundos " +
                    $"antes de volver a intentarlo.";
            }

            // Registrar la solicitud rechazada.
            var logger = httpContext.RequestServices
                .GetRequiredService<ILoggerFactory>()
                .CreateLogger("SGAT.Security.RateLimiting");

            logger.LogDebug(
                "Solicitud rechazada por Rate Limiting. Método: {Metodo}, Endpoint: {Endpoint}",
                httpContext.Request.Method,
                httpContext.GetEndpoint()?.DisplayName);

            await response.WriteAsync(
                mensaje,
                cancellationToken);
        }
    }
}