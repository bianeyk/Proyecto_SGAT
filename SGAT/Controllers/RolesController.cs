using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGAT.Application.Interfaces.Services;
using SGAT.Web.ViewModels.Rol;

namespace SGAT.Web.Controllers
{
    [Authorize(Roles = "Administrador")]
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
        [HttpGet]
        public IActionResult Create()
        {
            var modelo = new RolCreateViewModel();

            return View(modelo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RolCreateViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            var resultado = await _rolService.CrearAsync(modelo.Rol);

            if (!resultado.Succeeded)
            {
                foreach (var error in resultado.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(modelo);
            }

            TempData["Success"] = "El rol se creó correctamente.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activar(string id)
        {
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
        public async Task<IActionResult> Inactivar(string id)
        {
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