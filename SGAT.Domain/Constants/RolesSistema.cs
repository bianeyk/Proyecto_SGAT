namespace SGAT.Domain.Constants
{
    public static class RolesSistema
    {
        public const string Administrador = "Administrador";
        public const string Gerente = "Gerente";
        public const string Tecnico = "Tecnico";

        public const string Todos =
            Administrador + "," + Gerente + "," + Tecnico;

        public static IReadOnlyList<string> Nombres { get; } =
            Array.AsReadOnly(new[]
            {
                Administrador,
                Gerente,
                Tecnico
            });

        public static bool EsRolPermitido(string? nombre)
        {
            return Nombres.Contains(
                nombre,
                StringComparer.Ordinal);
        }

        public static bool EsRolReservado(string? nombre)
        {
            return Nombres.Contains(
                nombre,
                StringComparer.OrdinalIgnoreCase);
        }
    }
}