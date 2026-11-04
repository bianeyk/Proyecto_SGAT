using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGAT.Application.DTOs.Cliente;
using SGAT.Application.Interfaces.Services;
using SGAT.Web.ViewModels.Cliente;

namespace SGAT.Web.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class ClientesController : Controller
    {
        private readonly IClienteService _clienteService;

        public ClientesController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        public async Task<IActionResult> Index(string? termino)
        {
            var clientes = string.IsNullOrWhiteSpace(termino)
                ? await _clienteService.GetTodosAsync()
                : await _clienteService.BuscarAsync(termino.Trim());

            ViewData["Termino"] = termino;
            return View(clientes);
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
                var cliente = await _clienteService.CrearAsync(
                    new CrearClienteDto
                    {
                        NumDocumento = vm.NumDocumento,
                        Nombre = vm.Nombre,
                        Telefono = vm.Telefono,
                        Email = vm.Email,
                        Direccion = vm.Direccion,
                        EsColaborador = vm.EsColaborador,
                        Activo = vm.Activo
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
                await _clienteService.ActualizarAsync(id, new ActualizarClienteDto
                {
                    Nombre = vm.Nombre,
                    Telefono = vm.Telefono,
                    Email = vm.Email,
                    Direccion = vm.Direccion,
                    EsColaborador = vm.EsColaborador,
                    Activo = vm.Activo
                });

                TempData["Success"] = "Cliente actualizado correctamente.";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var cliente = await _clienteService.GetByIdAsync(id);
            return cliente is null ? NotFound() : View(cliente);
        }

        [HttpPost, ActionName("Delete")]
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