
namespace SGAT.Application.Interfaces.Services
{
    public interface IEmailService
    {
        //Agregamos el contrato
        Task EnviarCorreoAsync(
            string destinatario,
            string asunto,
            string mensaje);
    }
}
