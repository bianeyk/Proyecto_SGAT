using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SGAT.Application.DTOs.Usuario;
using SGAT.Application.Interfaces.Services;
using SGAT.Web.ViewModels.Usuario;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SGAT.Web.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class UsuariosController : Controller
    {
        //Inyectar dependencias
        private readonly IUsuarioService _usuarioService;
        private readonly IRolService _rolService;
        public UsuariosController(IUsuarioService usuarioService, IRolService rolService)
        {
            _usuarioService = usuarioService;
            _rolService = rolService;
        }
        //Index
        public async Task<IActionResult> Index()
        {
            var usuarios = await _usuarioService.ObtenerTodosAsync();

            return View(usuarios);
        }
        //Deatils
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            var usuarios = await _usuarioService.ObtenerPorIdAsync(id);

            if (usuarios == null)
            {
                return NotFound();
            }

            return View(usuarios);
        }
        //Metodo GET Create
        public async Task<IActionResult> Create()
        {
            var vm = new UsuarioCreateViewModel
            {
                RolesDisponibles = await _rolService.ObtenerTodosAsync()
            };

            return View(vm);
        }
        //Create POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UsuarioCreateViewModel vm)
        {
            //Validar ModelState
            if (!ModelState.IsValid)
            {
                vm.RolesDisponibles = await _rolService.ObtenerTodosAsync();

                return View(vm);
            }
            //Llamar al servicio
            var resultado = await _usuarioService.CrearAsync(vm.Usuario);
            //errores
            if (!resultado.Succeeded)
            {
                //Agregar los errores al ModelState
                foreach (var error in resultado.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                //Volver a cargar los roles
                vm.RolesDisponibles = await _rolService.ObtenerTodosAsync();

                return View(vm);
            }
            //mensaje Todo salió bien
            TempData["Success"] = "Usuario creado correctamente.";

            return RedirectToAction(nameof(Index));
        }
        //GET Edit
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            var usuario = await _usuarioService.ObtenerPorIdAsync(id);

            if (usuario == null)
                return NotFound();

            var vm = new UsuarioEditViewModel
            {
                Usuario = new UsuarioEditDto
                {
                    Id = usuario.Id,
                    NombreCompleto = usuario.NombreCompleto,
                    Activo = usuario.Activo,
                    Roles = usuario.Roles
                },
                RolesDisponibles = await _rolService.ObtenerTodosAsync()
            };

            return View(vm);
        }
        //POST Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UsuarioEditViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.RolesDisponibles = await _rolService.ObtenerTodosAsync();
                return View(vm);
            }

            var resultado = await _usuarioService.ActualizarAsync(vm.Usuario);

            if (!resultado.Succeeded)
            {
                foreach (var error in resultado.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                vm.RolesDisponibles = await _rolService.ObtenerTodosAsync();

                return View(vm);
            }

            TempData["Success"] = "Usuario actualizado correctamente.";

            return RedirectToAction(nameof(Index));
        }
        //Inactivar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Inactivar(string id)
        {
            var resultado = await _usuarioService.InactivarAsync(id);

            if (!resultado.Succeeded)
            {
                TempData["Error"] = resultado.Errors.First().Description;
            }
            else
            {
                TempData["Success"] = "Usuario inactivado correctamente.";
            }

            return RedirectToAction(nameof(Index));
        }
        //Activar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activar(string id)
        {
            var resultado = await _usuarioService.ActivarAsync(id);

            if (!resultado.Succeeded)
            {
                TempData["Error"] = resultado.Errors.First().Description;
            }
            else
            {
                TempData["Success"] = "Usuario activado correctamente.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
