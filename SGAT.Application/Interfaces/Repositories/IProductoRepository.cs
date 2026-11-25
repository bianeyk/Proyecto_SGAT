using SGAT.Domain.Entities;
using SGAT.Application.DTOs.Paginacion;
using System;
using System.Collections.Generic;
using System.Text;

namespace SGAT.Application.Interfaces.Repositories
{
    public interface IProductoRepository : IGenericRepository<Producto>
    {
        Task<Producto?> GetByCodigoAsync(string codigo);
        Task<bool> CodigoExisteAsync(string codigo);

        Task<PagedResult<Producto>> GetPageConDetalleAsync(
            string? termino, string filtro, int pagina, int tamanoPagina);
        Task<Producto?> GetByIdConDetalleAsync(int id);

        // Catálogos activos para los desplegables del formulario.
        Task<IEnumerable<CategoriaProducto>> GetCategoriasActivasAsync();
        Task<IEnumerable<Marca>> GetMarcasActivasAsync();
        Task<bool> CategoriaExisteAsync(int idCategoria);
        Task<bool> MarcaExisteAsync(int idMarca);
    }
}
