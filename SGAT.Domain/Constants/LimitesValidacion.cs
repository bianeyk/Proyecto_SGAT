namespace SGAT.Domain.Constants
{
    public static class LimitesValidacion
    {
        // Usuarios
        public const int NombreUsuarioMaximo = 100;
        public const int EmailMaximo = 256;

        // Roles
        public const int NombreRolMaximo = 50;

        // Identificadores de Identity
        public const int IdentityIdMaximo = 450;

        // Contraseñas
        public const int PasswordMinimo = 8;
        public const int PasswordMaximo = 100;
    }
}