using SGAT.Domain.Entities;
using SGAT.Domain.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGAT.Application.Interfaces.Repositories
{
    public interface ITecnicoRepository
    {
        Task<IEnumerable<OrdenServicio>> GetOrdenesAsync(EstadoOrden? estado = null, string? tecnicoId = null);
        Task<OrdenServicio?> GetDetalleOrdenAsync(int ordenId);
        Task<DiagnosticoTecnico?> GetDiagnosticoPorOrdenIdAsync(int ordenId);
        Task AddDiagnosticoAsync(DiagnosticoTecnico diagnostico);
        void UpdateDiagnostico(DiagnosticoTecnico diagnostico);
        Task AddBitacoraAsync(BitacoraTecnica bitacora);
        void UpdateOrden(OrdenServicio orden);
        Task<bool> SaveChangesAsync();
    }
}
