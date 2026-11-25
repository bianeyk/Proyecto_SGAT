using SGAT.Application.DTOs.Cliente;
using SGAT.Application.DTOs.Paginacion;
using System;
using System.Collections.Generic;
using System.Text;

namespace SGAT.Application.Interfaces.Services
{
    public interface IClienteService
    {
        Task<IEnumerable<ClienteDto>> GetTodosAsync();
        Task<ClienteDto?> GetByIdAsync(int id);
        Task<ClienteDto?> GetByNumDocAsync(string numDoc);
        Task<IEnumerable<ClienteDto>> BuscarAsync(string termino);
        Task<PagedResult<ClienteDto>> GetPageAsync(string? termino, int pagina, int tamanoPagina);
        Task<ClienteDto> CrearAsync(CrearClienteDto dto, string idUsuarioCrea);
        Task<ClienteDto> ActualizarAsync(int id, ActualizarClienteDto dto);
    }
}
