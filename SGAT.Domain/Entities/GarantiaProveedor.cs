using SGAT.SGAT.Domain.Enums;
namespace SGAT.Domain.Entities
{
    public class GarantiaProveedor
    {
        public int IdGarantiaProveedor { get; set; }

        public int IdLoteProducto { get; set; }
        public int IdProveedor { get; set; }

        public DateTime? FechaEnvio { get; set; }
        public DateTime? FechaRespuesta { get; set; }
        public EstadoGarantiaProveedorEnum? Estado { get; set; }
        public string? Observacion { get; set; }

        // Navegacion
        public LoteProducto LoteProducto { get; set; } = null!;
        public Proveedor Proveedor { get; set; } = null!;
    }
}
