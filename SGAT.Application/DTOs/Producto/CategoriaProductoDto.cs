namespace SGAT.Application.DTOs.Producto
{
    public class CategoriaProductoDto
    {
        public int IdCategoriaProducto { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string? Descripcion { get; set; }

        public bool Activo { get; set; } = true;
    }
}
