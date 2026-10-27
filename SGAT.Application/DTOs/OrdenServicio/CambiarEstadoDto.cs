using System;
using System.Collections.Generic;
using System.Text;

namespace SGAT.Application.DTOs.OrdenServicio
{
    public class CambiarEstadoDto
    {
        public int IdEstadoServicio { get; set; }
        public string? Comentario { get; set; }
    }
}
