using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SGAT.Application.DTOs.Cliente;
using SGAT.Application.Interfaces.Services;
using SGAT.Domain.Constants;
using SGAT.Web.Security;
using SGAT.Web.ViewModels.Cliente;
using System.Security.Claims;

namespace SGAT.Web.Controllers
{
    [Authorize(Roles = RolesSistema.Todos)]
    public class ClientesController : Controller
    {
        private readonly IClienteService _clienteService;

        // Número máximo de registros por página. Ajustar si se necesita más.
        private const int TamanoPagina = 25;
        private const int MaxPagina = 10_000;
        private const int MinLongitudBusqueda = 2;
        private const int MaxLongitudBusqueda = 100;

        public ClientesController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        [EnableRateLimiting(RateLimitPolicies.Search)]
        public async Task<IActionResult> Index(string? termino, int pagina = 1)
        {
            // Forzar límites de paginación para evitar descargas masivas.
            // La página se limita antes de enviarla al repositorio.
            pagina = Math.Clamp(pagina, 1, MaxPagina);
            termino = termino?.Trim();
            if (!string.IsNullOrWhiteSpace(termino)
                && termino.Length < MinLongitudBusqueda)
            {
                return BadRequest("El término de búsqueda debe tener al menos 2 caracteres.");
            }

            if (termino?.Length > MaxLongitudBusqueda)
                return BadRequest("El término de búsqueda no puede superar los 100 caracteres.");

            var result = await _clienteService.GetPageAsync(
                string.IsNullOrWhiteSpace(termino) ? null : termino,
                pagina,
                TamanoPagina);

            ViewData["Termino"] = termino;
            ViewData["TotalRegistros"] = result.TotalCount;
            ViewData["TotalActivos"] = result.ActiveCount;
            ViewData["TotalInactivos"] = result.InactiveCount;
            ViewData["PaginaActual"] = pagina;
            ViewData["TotalPaginas"] = Math.Max(1,
                (int)Math.Ceiling(result.TotalCount / (double)TamanoPagina));
            return View(result.Items);
        }

        public async Task<IActionResult> Details(int id)
        {
            var cliente = await _clienteService.GetByIdAsync(id);
            return cliente is null ? NotFound() : View(cliente);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new ClienteViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClienteViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var idUsuarioCrea = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(idUsuarioCrea))
                return Forbid();

            try
            {
                // Solo gerente o administrador puede definir estos indicadores.
                // Nunca se deben aceptar directamente desde un formulario de técnico.
                var puedeGestionarEstado =
                    User.IsInRole("Gerente") || User.IsInRole("Administrador");
                var cliente = await _clienteService.CrearAsync(
                    new CrearClienteDto
                    {
                        NumDocumento = vm.NumDocumento.Trim(),
                        Nombre = vm.Nombre.Trim(),
                        Telefono = vm.Telefono.Trim(),
                        Email = vm.Email?.Trim(),
                        Direccion = vm.Direccion?.Trim(),
                        EsColaborador = puedeGestionarEstado && vm.EsColaborador,
                        Activo = puedeGestionarEstado ? vm.Activo : true
                    },
                    idUsuarioCrea);

                TempData["Success"] = "Cliente creado correctamente.";
                return RedirectToAction(nameof(Details), new { id = cliente.IdCliente });
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(nameof(vm.NumDocumento), ex.Message);
                return View(vm);
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(nameof(vm.NumDocumento), ex.Message);
                return View(vm);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var cliente = await _clienteService.GetByIdAsync(id);
            if (cliente is null)
                return NotFound();

            return View(new ClienteViewModel
            {
                IdCliente = cliente.IdCliente,
                NumDocumento = cliente.NumDocumento,
                Nombre = cliente.Nombre,
                Telefono = cliente.Telefono,
                Email = cliente.Email,
                Direccion = cliente.Direccion,
                EsColaborador = cliente.EsColaborador,
                Activo = cliente.Activo
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ClienteViewModel vm)
        {
            if (id != vm.IdCliente)
                return NotFound();

            var clienteActual = await _clienteService.GetByIdAsync(id);
            if (clienteActual is null)
                return NotFound();

            // El documento no se envía en el formulario de edición y no se puede actualizar.
            ModelState.Remove(nameof(vm.NumDocumento));
            if (!ModelState.IsValid)
            {
                vm.NumDocumento = clienteActual.NumDocumento;
                return View(vm);
            }

            try
            {
                // Para técnicos, conservar los valores actuales aunque el request
                // incluya campos manipulados de forma manual.
                var puedeGestionarEstado =
                    User.IsInRole("Gerente") || User.IsInRole("Administrador");
                await _clienteService.ActualizarAsync(id, new ActualizarClienteDto
                {
                    Nombre = vm.Nombre,
                    Telefono = vm.Telefono,
                    Email = vm.Email,
                    Direccion = vm.Direccion,
                    EsColaborador = puedeGestionarEstado
                        ? vm.EsColaborador
                        : clienteActual.EsColaborador,
                    Activo = puedeGestionarEstado ? vm.Activo : clienteActual.Activo
                });

                TempData["Success"] = "Cliente actualizado correctamente.";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (ArgumentException ex)
            {
                vm.NumDocumento = clienteActual.NumDocumento;
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(vm);
            }
        }

        [HttpGet]
        [Authorize(Roles = "Administrador,Gerente")]
        public async Task<IActionResult> Delete(int id)
        {
            var cliente = await _clienteService.GetByIdAsync(id);
            return cliente is null ? NotFound() : View(cliente);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Administrador,Gerente")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cliente = await _clienteService.GetByIdAsync(id);
            if (cliente is null)
                return NotFound();

            try
            {
                await _clienteService.ActualizarAsync(id, new ActualizarClienteDto
                {
                    Nombre = cliente.Nombre,
                    Telefono = cliente.Telefono,
                    Email = cliente.Email,
                    Direccion = cliente.Direccion,
                    EsColaborador = cliente.EsColaborador,
                    Activo = false
                });

                TempData["Success"] = "Cliente dado de baja correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
