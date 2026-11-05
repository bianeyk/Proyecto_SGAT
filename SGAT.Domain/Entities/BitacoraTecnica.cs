using System;

namespace SGAT.Domain.Entities
{
    public class BitacoraTecnica
    {
        public int Id { get; set; }

        public int OrdenId { get; set; }
        public string Nota { get; set; } = string.Empty;

        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

        // Navegación
        public OrdenServicio Orden { get; set; } = null!;
    }
}
