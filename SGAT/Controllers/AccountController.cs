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

        public AccountController(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginDto model)
        {
            if (!ModelState.IsValid)
                return View(model);
            // 1. Intentar iniciar sesión en Identity
            var result = await _signInManager.PasswordSignInAsync(
                model.UserName,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                // 2. Buscar al usuario en la base de datos usando el UserName (que es su Email)
                var user = await _userManager.FindByNameAsync(model.UserName);

                if (user != null)
                {
                    // 3. Validar si el usuario tiene asignado el rol exacto "Administrador"
                    var isAdmin = await _userManager.IsInRoleAsync(user, "Administrador");

                    if (isAdmin)
                    {
                        // Redirigir al Index de UsuariosController (Nota la S al final: "Usuarios")
                        return RedirectToAction("Index", "Usuarios");
                    }
                }

                // 4. Redirección por defecto si no es administrador (ej. la página de inicio)
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
