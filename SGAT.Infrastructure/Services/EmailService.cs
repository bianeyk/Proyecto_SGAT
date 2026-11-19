
using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client.NativeInterop;
using MimeKit;
using SGAT.Application.Interfaces.Services;
using SGAT.Infrastructure.Settings;

namespace SGAT.Infrastructure.Services
{
    //implementa el envío mediante MailKit
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;

        //Recibe la configuración del correo
        public EmailService(IOptions<EmailSettings> options)
        {
            _settings = options.Value;
        }

        public async Task EnviarCorreoAsync(
            string destinatario,
            string asunto,
            string mensaje)
        {
            //Construye el mensaje que vamos a enviar
            var email = new MimeMessage();

            email.From.Add(
                new MailboxAddress(
                    _settings.FromName,
                    _settings.FromEmail));
            //Representa las direcciones de correo
            email.To.Add(
                MailboxAddress.Parse(destinatario));

            email.Subject = asunto;

            email.Body = new TextPart("plain")
            {
                Text = mensaje
            };

            //Establece la conexión con el servidor SMTP
            using var smtp = new SmtpClient();

            try
            {
                //Se conecta al servidor utilizando STARTTLS
                await smtp.ConnectAsync(
                    _settings.Host,
                    _settings.Port,
                    _settings.SecurityOption);
                //Autentica la cuenta que enviará el correo
                await smtp.AuthenticateAsync(
                    _settings.UserName,
                    _settings.Password);
                //Envía el mensaje.
                await smtp.SendAsync(email);
            }
            finally
            {//comprueba si todavía existe una conexión SMTP.
                if (smtp.IsConnected)
                {
                    //Cierra la conexión con el servidor SMTP
                    await smtp.DisconnectAsync(true);
                }
            }

        }
    }
}
