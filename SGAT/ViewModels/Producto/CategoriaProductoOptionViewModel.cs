namespace SGAT.Web.ViewModels.Producto
{
    /// <summary>
    /// Opción de categoría usada en el desplegable del formulario de productos.
    /// </summary>
    public class CategoriaProductoOptionViewModel
    {
        public int IdCategoriaProducto { get; set; }

        public string Nombre { get; set; } = string.Empty;
    }
}
