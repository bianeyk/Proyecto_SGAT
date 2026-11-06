using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SGAT.Application.DTOs.Login;
using SGAT.Domain.Entities;

namespace SGAT.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public AccountController(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public IActionResult Login(bool inactiva = false, bool permisos = false)
        {
            if (inactiva)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Tu cuenta está inactivada. Comunícate con el administrador.");
            }
            else if (permisos)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Tus permisos han cambiado. Inicia sesión nuevamente.");
            }

            return View();
        }

        //consultar si el rol está activo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // 1. Buscar al usuario antes de iniciar sesión.
            var user = await _userManager.FindByNameAsync(model.UserName);

            // Si no existe, mostrar un mensaje genérico.
            if (user == null)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Usuario o contraseña incorrectos");

                return View(model);
            }

            // 2. Comprobar si el usuario está activo.
            if (!user.Activo)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Tu cuenta está inactivada. Comunícate con el administrador.");

                return View(model);
            }

            // 3. Obtener los roles asignados al usuario.
            var roles = await _userManager.GetRolesAsync(user);

            // El sistema espera que cada usuario tenga un único rol.
            if (roles.Count != 1)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Tu cuenta está inactivada. Comunícate con el administrador.");

                return View(model);
            }

            // 4. Buscar el rol y comprobar si está activo.
            var rol = await _roleManager.FindByNameAsync(roles[0]);

            if (rol == null || !rol.Activo)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Tu cuenta está inactivada. Comunícate con el administrador.");

                return View(model);
            }

            // 5. Solo ahora validar la contraseña e iniciar sesión.
            var result = await _signInManager.PasswordSignInAsync(
                user,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var isAdmin = await _userManager.IsInRoleAsync(
                    user,
                    "Administrador");

                if (isAdmin)
                {
                    return RedirectToAction("Index", "Usuarios");
                }

                return RedirectToAction("AccessDenied", "Account");
            }

            ModelState.AddModelError(
                string.Empty,
                "Usuario o contraseña incorrectos");

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction(
                "Login",
                "Account");
        }
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
