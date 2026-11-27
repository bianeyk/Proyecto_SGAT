using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SGAT.Application.DTOs.Usuario;
using SGAT.Application.Interfaces.Services;
using SGAT.Web.ViewModels.Usuario;
using SGAT.Application.DTOs.Paginacion;
using Microsoft.AspNetCore.RateLimiting;
using SGAT.Web.Security;
using SGAT.Application.Validators;
using SGAT.Domain.Constants;
using SGAT.Application.DTOs.Rol;


namespace SGAT.Web.Controllers
{
    [Authorize(Roles = RolesSistema.Administrador)]
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
        //Get Index
        [HttpGet]
        [EnableRateLimiting(RateLimitPolicies.Search)]
        public async Task<IActionResult> Index([FromQuery] UsuarioFiltroDto filtro)
        {
            // 1. Validar el Model Binding.
            if (!ModelState.IsValid)
            {
                return BadRequest("Los parámetros de búsqueda no son válidos.");
            }

            // 2. Establecer límites de paginación.
            filtro.Pagina = PaginacionHelper.NormalizarPagina(filtro.Pagina);

            filtro.TamanoPagina = PaginacionHelper.NormalizarTamano(filtro.TamanoPagina);

            // 3. Preparar el ViewModel.
            var vm = new UsuarioIndexViewModel
            {
                Filtro = filtro,
                RolesDisponibles = await _rolService.ObtenerTodosAsync()
            };

            try
            {
                // 4. Consultar los usuarios paginados.
                vm.Resultado = await _usuarioService
                    .ObtenerPaginadosAsync(filtro);

                // 5. Corregir páginas fuera del rango disponible.
                if (filtro.Pagina > vm.TotalPaginas)
                {
                    filtro.Pagina = vm.TotalPaginas;

                    vm.Resultado = await _usuarioService
                        .ObtenerPaginadosAsync(filtro);
                }
            }
            catch (ArgumentException ex)
            {
                // 6. Mostrar errores de búsqueda.
                ModelState.AddModelError(string.Empty, ex.Message);

                vm.Resultado = new PagedResult<UsuarioListDto>();
            }

            // 7. Enviar el ViewModel a la vista.
            return View(vm);
        }

        //Get Details
        [HttpGet]
        public async Task<IActionResult> Details([FromRoute] string id)
        {
            if (!IdentityIdValidator.EsValido(id))
            {
                return BadRequest("El identificador no es válido.");
            }

            var usuario = await _usuarioService.ObtenerPorIdAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        //Metodo GET Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var vm = new UsuarioCreateViewModel
            {
                RolesDisponibles =
                    await ObtenerRolesAsignablesAsync()
            };

            return View(vm);
        }

        //Create POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] UsuarioCreateViewModel vm)
        {
            // 1. Validar los datos del formulario.
            if (!ModelState.IsValid)
            {
                vm.RolesDisponibles =
                    await ObtenerRolesAsignablesAsync();

                return View(vm);
            }

            // 2. Mapear ViewModel a DTO.
            var dto = new UsuarioCreateDto
            {
                NombreCompleto = vm.NombreCompleto,
                Email = vm.Email,
                Password = vm.Password,
                ConfirmPassword = vm.ConfirmPassword,
                Rol = vm.Rol
            };

            // 3. Llamar al servicio.
            var resultado = await _usuarioService.CrearAsync(dto);

            // 4. Gestionar errores del servicio.
            if (!resultado.Succeeded)
            {
                foreach (var error in resultado.Errors)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        error.Description);
                }

                vm.RolesDisponibles =
                    await ObtenerRolesAsignablesAsync();

                return View(vm);
            }

            // 5. Confirmar la operación.
            TempData["Success"] = "Usuario creado correctamente.";

            return RedirectToAction(nameof(Index));
        }

        //GET Edit
        [HttpGet]
        public async Task<IActionResult> Edit([FromRoute] string id)
        {
            // 1. Validar identificador.
            if (!IdentityIdValidator.EsValido(id))
            {
                return BadRequest();
            }

            // 2. Obtener usuario.
            var usuario =
                await _usuarioService.ObtenerPorIdAsync(id);

            if (usuario is null)
            {
                return NotFound();
            }

            // 3. Mapear DTO a ViewModel.
            var vm = new UsuarioEditViewModel
            {
                Id = usuario.Id,
                NombreCompleto = usuario.NombreCompleto,
                Activo = usuario.Activo,
                Rol = usuario.Roles.FirstOrDefault()
                    ?? string.Empty,

                RolesDisponibles =
                    await ObtenerRolesAsignablesAsync()
            };

            // 4. Mostrar formulario.
            return View(vm);
        }

        //POST Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute] string id,[FromForm] UsuarioEditViewModel vm)
        {
            // 1. Validar identificadores.
            if (vm is null ||
                !IdentityIdValidator.EsValido(id) ||
                !IdentityIdValidator.EsValido(vm.Id) ||
                !string.Equals(
                    id,
                    vm.Id,
                    StringComparison.Ordinal))
            {
                return BadRequest();
            }

            // 2. Validar datos del formulario.
            if (!ModelState.IsValid)
            {
                vm.RolesDisponibles =
                    await ObtenerRolesAsignablesAsync();

                return View(vm);
            }

            // 3. Mapear ViewModel a DTO.
            var dto = new UsuarioEditDto
            {
                Id = id,
                NombreCompleto = vm.NombreCompleto,
                Activo = vm.Activo,
                Rol = vm.Rol
            };

            // 4. Llamar al servicio.
            var resultado =
                await _usuarioService.ActualizarAsync(dto);

            // 5. Gestionar errores del servicio.
            if (!resultado.Succeeded)
            {
                foreach (var error in resultado.Errors)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        error.Description);
                }

                vm.RolesDisponibles =
                    await ObtenerRolesAsignablesAsync();

                return View(vm);
            }

            // 6. Confirmar la operación.
            TempData["Success"] = "Usuario actualizado correctamente.";

            return RedirectToAction(nameof(Index));
        }

        //POST Usuarios/Inactivar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Inactivar([FromRoute] string id)
        {
            if (!IdentityIdValidator.EsValido(id))
            {
                return BadRequest("El identificador no es válido.");
            }
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }
            var resultado = await _usuarioService.InactivarAsync(id);

            if (!resultado.Succeeded)
            {
                TempData["Error"] = string.Join(", ", resultado.Errors.Select(e => e.Description));
            }
            else
            {
                TempData["Success"] = "Usuario inactivado correctamente.";
            }

            return RedirectToAction(nameof(Index));
        }

        //POST Activar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activar([FromRoute] string id)
        {
            if (!IdentityIdValidator.EsValido(id))
            {
                return BadRequest("El identificador no es válido.");
            }
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }
            var resultado = await _usuarioService.ActivarAsync(id);

            if (!resultado.Succeeded)
            {
                TempData["Error"] = string.Join(", ", resultado.Errors.Select(e => e.Description));
            }
            else
            {
                TempData["Success"] = "Usuario activado correctamente.";
            }

            return RedirectToAction(nameof(Index));
        }
        private async Task<IReadOnlyList<RolDto>>ObtenerRolesAsignablesAsync()
        {
            return (await _rolService.ObtenerTodosAsync())
                .Where(r => r.Activo &&
                    RolesSistema.EsRolPermitido(r.Nombre))
                .ToList();
        }
    }
}
