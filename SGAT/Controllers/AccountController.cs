using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Logging;
using SGAT.Application.DTOs.Login;
using SGAT.Application.Interfaces.Services;
using SGAT.Domain.Entities;
using SGAT.Web.ViewModels.Account;


namespace SGAT.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IEmailService _emailService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IEmailService emailService,
            ILogger<AccountController> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _emailService = emailService;
            _logger = logger;
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
        [EnableRateLimiting("LoginPolicy")]
        public async Task<IActionResult> Login(LoginDto model)
        {
            if (!ModelState.IsValid)
                return View(model);
            // 1. Buscar al usuario antes de iniciar sesión.
            var user = await _userManager.FindByNameAsync(
                model.UserName);
            // validar Si no existe.
            if (user == null)
                return LoginInvalido(model);

            // Validar contraseña sin iniciar sesión.
            var resultado = await _signInManager.CheckPasswordSignInAsync(
                user,
                model.Password,
                lockoutOnFailure: true);

            if (!resultado.Succeeded)
                return LoginInvalido(model);

            // 2. Comprobar si el usuario está activo.
            if (!user.Activo)
                return LoginInvalido(model);

            // 3. Comprobar que tenga exactamente un rol.
            var roles = await _userManager.GetRolesAsync(user);
            // El sistema espera que cada usuario tenga un único rol.
            if (roles.Count != 1)
                return LoginInvalido(model);

            // 4. Buscar el rol y comprobar si está activo.
            var rol = await _roleManager.FindByNameAsync(roles[0]);

            if (rol == null || !rol.Activo)
                return LoginInvalido(model);

            // 5. Todas las validaciones fueron satisfactorias.
            await _signInManager.SignInAsync(
                user,
                model.RememberMe);

            // 6. Redireccionar según el rol.
            if (roles[0] == "Administrador")
                return RedirectToAction("Index", "Usuarios");

            if (roles[0] == "Gerente")
                return RedirectToAction("Index", "Clientes");

            if (roles[0] == "Tecnico")
                return RedirectToAction("Index", "Clientes");

            return RedirectToAction("AccessDenied", "Account");
        }

        private IActionResult LoginInvalido(LoginDto model)
        {
            ModelState.AddModelError(
                string.Empty,
                "Usuario o contraseña incorrectos.");

            return View("Login", model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Login", "Account");
        }
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EnableRateLimiting("ForgotPasswordPolicy")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var usuario = await _userManager.FindByEmailAsync(
                model.Email.Trim());

            if (usuario == null || !usuario.Activo)
            {
                return RedirectToAction(
                    nameof(ForgotPasswordConfirmation));
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(
                usuario);

            var enlace = Url.Action(
                nameof(ResetPassword),
                "Account",
                new
                {
                    email = usuario.Email,
                    token = token
                },
                Request.Scheme);
            //Comprobar que el usuario tenga un correo válido
            if (string.IsNullOrWhiteSpace(enlace))
            {
                _logger.LogError(
                    "No fue posible generar el enlace de recuperación.");

                return RedirectToAction(
                    nameof(ForgotPasswordConfirmation));
            }

            try
            {
                var mensaje = $"""
        Hola {usuario.NombreCompleto},

        Hemos recibido una solicitud para restablecer
        tu contraseña de SGAT.

        Para establecer una nueva contraseña, ingresa al
        siguiente enlace:

        {enlace}

        Si no solicitaste este cambio, puedes ignorar
        este mensaje.

        Saludos,
        Equipo SGAT
        """;

                await _emailService.EnviarCorreoAsync(
                    usuario.Email!,
                    "Restablecer contraseña - SGAT",
                    mensaje);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error al enviar el correo de recuperación de contraseña.");
            }

            return RedirectToAction(
                nameof(ForgotPasswordConfirmation));
        }

        [HttpGet]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword(string email, string token)
        {
            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(token))
            {
                return RedirectToAction(nameof(Login));
            }

            var model = new ResetPasswordViewModel
            {
                Email = email,
                Token = token
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(
    ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var usuario = await _userManager.FindByEmailAsync(
                model.Email.Trim());

            if (usuario == null || !usuario.Activo)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "El enlace de recuperación no es válido o ha expirado.");

                return View(model);
            }
            //Security Stamp para proteger las operaciones de autenticación.
            var resultado = await _userManager.ResetPasswordAsync(
                usuario,
                model.Token,
                model.NewPassword);

            if (!resultado.Succeeded)
            {
                foreach (var error in resultado.Errors)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        error.Description);
                }

                return View(model);
            }

            return RedirectToAction(
                nameof(ResetPasswordConfirmation));
        }
    }
}
