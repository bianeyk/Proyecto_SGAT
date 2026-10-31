using SGAT.Application.DTOs.Tecnico;
using SGAT.Domain.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGAT.Application.Interfaces.Services
{
    public interface ITecnicoService
    {
        Task<IEnumerable<OrdenTecnicoResumenDto>> ObtenerOrdenesAsync(EstadoOrden? estado = null, string? tecnicoId = null);
        Task<DetalleOrdenTecnicoDto?> ObtenerDetalleOrdenAsync(int id);
        Task<bool> IniciarDiagnosticoAsync(IniciarDiagnosticoDto dto);
        Task<bool> RegistrarDiagnosticoAsync(RegistrarDiagnosticoDto dto);
        Task<bool> AgregarNotaBitacoraAsync(AgregarNotaBitacoraDto dto);
        Task<bool> FinalizarReparacionAsync(FinalizarReparacionDto dto);
    }
}
