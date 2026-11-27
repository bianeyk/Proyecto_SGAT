namespace SGAT.Application.Validators
{
    public static class IdentityIdValidator
    {
        public const int LongitudMaxima = 450;

        public static bool EsValido(string? id)
        {
            return !string.IsNullOrWhiteSpace(id)
                && id.Length <= LongitudMaxima
                && !id.Any(char.IsWhiteSpace)
                && !id.Any(char.IsControl);
        }
    }
}