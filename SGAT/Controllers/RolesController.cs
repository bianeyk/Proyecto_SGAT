using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGAT.Application.DTOs.Rol;
using SGAT.Application.Interfaces.Services;
using SGAT.Application.Validators;
using SGAT.Domain.Constants;
using SGAT.Web.ViewModels.Rol;


namespace SGAT.Web.Controllers
{
    [Authorize(Roles = RolesSistema.Administrador)]
    public class RolesController : Controller
    {
        private readonly IRolService _rolService;


        public RolesController(IRolService rolService)
        {
            _rolService = rolService;
        }

        public async Task<IActionResult> Index()
        {
            var roles = await _rolService.ObtenerTodosAsync();

            return View(roles);
        }
        // GET: Roles/Create
        [HttpGet]
        public IActionResult Create()
        {
            var modelo = new RolCreateViewModel();

            return View(modelo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] RolCreateViewModel modelo)
        {
            // 1. Validar los datos del formulario.
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            // 2. Mapear ViewModel a DTO.
            var dto = new RolCreateDto
            {
                Nombre = modelo.Nombre
            };

            // 3. Llamar al servicio.
            var resultado = await _rolService.CrearAsync(dto);

            // 4. Gestionar errores del servicio.
            if (!resultado.Succeeded)
            {
                foreach (var error in resultado.Errors)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        error.Description);
                }

                return View(modelo);
            }

            // 5. Confirmar la operación.
            TempData["Success"] = "El rol se creó correctamente.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]public async Task<IActionResult> Activar([FromForm] string id)
        {
            if (!IdentityIdValidator.EsValido(id))
            {
                return BadRequest("El identificador no es válido.");
            }
            var resultado = await _rolService.ActivarAsync(id);

            if (!resultado.Succeeded)
            {
                TempData["Error"] = string.Join(
                    ", ",
                    resultado.Errors.Select(e => e.Description));
            }
            else
            {
                TempData["Success"] = "El rol se activó correctamente.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Inactivar([FromForm] string id)
        {
            if (!IdentityIdValidator.EsValido(id))
            {
                return BadRequest("El identificador no es válido.");
            }
            var resultado = await _rolService.InactivarAsync(id);

            if (!resultado.Succeeded)
            {
                TempData["Error"] = string.Join(
                    ", ",
                    resultado.Errors.Select(e => e.Description));
            }
            else
            {
                TempData["Success"] = "El rol se inactivó correctamente.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}