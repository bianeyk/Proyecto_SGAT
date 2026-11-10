namespace SGAT.Web.ViewModels.Producto
{
    /// <summary>
    /// Opción de marca usada en el desplegable del formulario de productos.
    /// </summary>
    public class MarcaOptionViewModel
    {
        public int IdMarca { get; set; }

        public string Nombre { get; set; } = string.Empty;
    }
}
