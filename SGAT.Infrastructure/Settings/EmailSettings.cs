using MailKit.Security;

namespace SGAT.Infrastructure.Settings
{
    //clase que representa la configuración del servidor de correo.
    public class EmailSettings
    {
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; }
        public string Provider { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FromEmail { get; set; } = string.Empty;
        public string FromName { get; set; } = string.Empty;
        public SecureSocketOptions SecurityOption { get; set; } = SecureSocketOptions.StartTls;
    }
}
