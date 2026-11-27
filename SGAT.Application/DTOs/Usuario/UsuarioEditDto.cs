namespace SGAT.Application.DTOs.Usuario
{
    public class UsuarioEditDto
    {
        public string Id { get; set; } = string.Empty;

        public string NombreCompleto { get; set; } = string.Empty;

        public bool Activo { get; set; }

        public string Rol { get; set; } = string.Empty;
    }
}