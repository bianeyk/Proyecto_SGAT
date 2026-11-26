namespace SGAT.Application.DTOs.Paginacion
{
    public static class PaginacionHelper
    {
        public const int PaginaMinima = 1;
        public const int PaginaMaxima = 10_000;
        public const int TamanoPredeterminado = 25;

        public static int NormalizarPagina(int pagina)
        {
            return Math.Clamp(
                pagina,
                PaginaMinima,
                PaginaMaxima);
        }

        public static int NormalizarTamano(int tamano)
        {
            return tamano is 10 or 25 or 50 or 100
                ? tamano
                : TamanoPredeterminado;
        }
    }
}
