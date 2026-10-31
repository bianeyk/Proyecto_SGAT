using Microsoft.AspNetCore.Mvc;
using SGAT.Application.DTOs.Tecnico;
using SGAT.Application.Interfaces.Services;
using SGAT.Domain.Enums;
using System.Threading.Tasks;

namespace SGAT.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TecnicoController : Controller
    {
        private readonly ITecnicoService _tecnicoService;

        public TecnicoController(ITecnicoService tecnicoService)
        {
            _tecnicoService = tecnicoService;
        }

        // ==========================================
        // VISTAS HTML (Panel del Taller Técnico)
        // ==========================================

        /// <summary>
        /// Vista principal del módulo técnico con tablero y gestión de órdenes
        /// GET /Tecnico
        /// </summary>
        [HttpGet("/Tecnico")]
        [HttpGet("/Tecnico/Index")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Index()
        {
            var ordenes = await _tecnicoService.ObtenerOrdenesAsync();
            return View(ordenes);
        }

        /// <summary>
        /// Vista de detalle técnico de la orden
        /// GET /Tecnico/Detalle/{id}
        /// </summary>
        [HttpGet("/Tecnico/Detalle/{id:int}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Detalle(int id)
        {
            if (id <= 0) return NotFound();
            var detalle = await _tecnicoService.ObtenerDetalleOrdenAsync(id);
            if (detalle == null) return NotFound();
            return View(detalle);
        }

        // ==========================================
        // ENDPOINTS REST API
        // ==========================================

        /// <summary>
        /// 1. GET api/tecnico/ordenes
        /// Lista las órdenes asignadas o pendientes de revisión con filtros opcionales por estado o técnico.
        /// </summary>
        [HttpGet("ordenes")]
        public async Task<IActionResult> ObtenerOrdenes([FromQuery] EstadoOrden? estado, [FromQuery] string? tecnicoId)
        {
            var ordenes = await _tecnicoService.ObtenerOrdenesAsync(estado, tecnicoId);
            return Ok(ordenes);
        }

        /// <summary>
        /// 2. GET api/tecnico/ordenes/{id}
        /// Detalle técnico completo de un equipo y orden específica.
        /// </summary>
        [HttpGet("ordenes/{id:int}")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> ObtenerDetalleOrden(int id)
        {
            if (id <= 0)
            {
                return BadRequest("El identificador de la orden debe ser un número entero positivo.");
            }

            var detalle = await _tecnicoService.ObtenerDetalleOrdenAsync(id);
            if (detalle == null)
            {
                return NotFound("La orden especificada no existe");
            }

            return Ok(detalle);
        }

        /// <summary>
        /// Endpoint complementario para iniciar diagnóstico y asignar técnico a la orden
        /// POST api/tecnico/iniciar-diagnostico
        /// </summary>
        [HttpPost("iniciar-diagnostico")]
        public async Task<IActionResult> IniciarDiagnostico([FromBody] IniciarDiagnosticoDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var exito = await _tecnicoService.IniciarDiagnosticoAsync(dto);
            if (!exito)
            {
                return NotFound("La orden especificada no existe");
            }

            return Ok(new { mensaje = "Diagnóstico iniciado exitosamente y orden puesta en estado EnDiagnostico." });
        }

        /// <summary>
        /// 3. POST api/tecnico/diagnostico
        /// Registra o actualiza el diagnóstico técnico de una orden y cambia su estado automáticamente.
        /// </summary>
        [HttpPost("diagnostico")]
        public async Task<IActionResult> RegistrarDiagnostico([FromBody] RegistrarDiagnosticoDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var exito = await _tecnicoService.RegistrarDiagnosticoAsync(dto);
            if (!exito)
            {
                return NotFound("La orden especificada no existe");
            }

            return Ok(new
            {
                mensaje = "Diagnóstico técnico registrado satisfactoriamente.",
                ordenId = dto.OrdenId,
                nuevoEstado = dto.NuevoEstado.ToString()
            });
        }

        /// <summary>
        /// 4. POST api/tecnico/bitacora
        /// Agrega notas rápidas de avance a la orden sin cerrarla.
        /// </summary>
        [HttpPost("bitacora")]
        public async Task<IActionResult> AgregarNotaBitacora([FromBody] AgregarNotaBitacoraDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var exito = await _tecnicoService.AgregarNotaBitacoraAsync(dto);
            if (!exito)
            {
                return NotFound("La orden especificada no existe");
            }

            return StatusCode(201, new { mensaje = "Nota agregada a la bitácora técnica de la orden.", ordenId = dto.OrdenId });
        }

        /// <summary>
        /// 5. PUT api/tecnico/finalizar
        /// Cierra la reparación, calcula totales y marca el estado como Finalizado.
        /// </summary>
        [HttpPut("finalizar")]
        public async Task<IActionResult> FinalizarReparacion([FromBody] FinalizarReparacionDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var exito = await _tecnicoService.FinalizarReparacionAsync(dto);
            if (!exito)
            {
                return NotFound("La orden especificada no existe");
            }

            return Ok(new
            {
                mensaje = "Reparación finalizada con éxito. Orden marcada como Finalizado.",
                ordenId = dto.OrdenId,
                costoFinal = dto.CostoFinal
            });
        }
    }
}
