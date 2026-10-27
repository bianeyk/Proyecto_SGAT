using SGAT.Application.DTOs.OrdenServicio;
using System;
using System.Collections.Generic;
using System.Text;

namespace SGAT.Application.Interfaces.Services
{
    public interface IOrdenServicioService
    {
        Task<IEnumerable<OrdenServicioDto>> GetTodasAsync();
        Task<OrdenServicioDto?> GetByIdAsync(int id);
        Task<IEnumerable<OrdenServicioDto>> GetByTecnicoAsync(string idUsuario);
        Task<OrdenServicioDto> CrearAsync(CrearOrdenServicioDto dto, string idUsuarioCrea);
        Task<bool> CambiarEstadoAsync(int id, CambiarEstadoDto dto, string idUsuario);
    }
}
