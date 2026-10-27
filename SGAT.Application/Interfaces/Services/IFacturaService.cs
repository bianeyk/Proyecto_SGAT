using SGAT.Application.DTOs.Factura;
using System;
using System.Collections.Generic;
using System.Text;

namespace SGAT.Application.Interfaces.Services
{
    public interface IFacturaService
    {
        Task<IEnumerable<FacturaDto>> GetTodasAsync();
        Task<FacturaDto?> GetByIdAsync(int id);
        Task<IEnumerable<FacturaDto>> GetPendientesAsync();
        Task<FacturaDto> CrearAsync(CrearFacturaDto dto, string idUsuarioCrea);
        Task<bool> AnularAsync(int id, string idUsuario);
        Task<bool> RegistrarPagoAsync(int idFactura, decimal abono, string metodoPago, string idUsuario);
    }
}

