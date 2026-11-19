using Microsoft.AspNetCore.Authorization;
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
            var user = await _userManager.FindByNameAsync(model.UserName);
            // validar Si no existe.
            if (user == null)
                return LoginInvalido(model);

            // Validar contraseña sin iniciar sesión.
            var resultado = await _signInManager.CheckPasswordSignInAsync(
                user,
                model.Password,
                lockoutOnFailure: true);
            // Validar si el usuario está bloqueado.
            if (resultado.IsLockedOut)
            {
                return LoginInvalido(model);
            }
            // Validar si la contraseña es incorrecta.
            // Comprobar si Identity bloqueó temporalmente la cuenta.
            if (resultado.IsLockedOut)
            {
                model.Password = string.Empty;

                ModelState.AddModelError(
                    string.Empty,
                    "Demasiados intentos de inicio de sesión. " +
                    "Por seguridad, el acceso se ha limitado temporalmente. " +
                    "Espera unos minutos antes de volver a intentarlo.");

                return View("Login", model);
            }

            // Mantener el mensaje genérico para los demás errores.
            if (!resultado.Succeeded)
            {
                return LoginInvalido(model);
            }

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
            // 5. Comprobar si el rol es uno de los permitidos.
            var esRolPermitido =
                roles[0] == "Administrador" ||
                roles[0] == "Gerente" ||
                roles[0] == "Tecnico";

            if (!esRolPermitido)
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
            // 1. Limpiar la contraseña.
            model.Password = string.Empty;

            // 2. Mostrar un mensaje genérico.
            ModelState.AddModelError(
                string.Empty,
                "Usuario o contraseña incorrectos.");

            // 3. Volver al formulario.
            return View("Login", model);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            //cierra la sesión de Identity y elimina la cookie de autenticación correspondiente
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
            // Buscar al usuario por correo electrónico.
            var usuario = await _userManager.FindByEmailAsync(
                model.Email.Trim());

            if (usuario == null || !usuario.Activo)
            {
                return RedirectToAction(
                    nameof(ForgotPasswordConfirmation));
            }
            // Generar un token de restablecimiento de contraseña.
            var token = await _userManager.GeneratePasswordResetTokenAsync(
                usuario);
            // Crear un enlace de restablecimiento de contraseña.
            var enlace = Url.Action(
                nameof(ResetPassword),
                "Account",
                new
                {
                    email = usuario.Email,
                    token = token
                },
                //permite generar el enlace con el protocolo de la solicitud
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
        //confirma que se procesó la solicitud de recuperación
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
        //confirma que se procesó la solicitud de restablecimiento de contraseña
        [HttpGet]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            // 1. Validar los datos recibidos.
            if (!ModelState.IsValid)
            {
                model.NewPassword = string.Empty;
                model.ConfirmPassword = string.Empty;

                return View(model);
            }

            // 2. Buscar al usuario por correo electrónico.
            var usuario = await _userManager.FindByEmailAsync(
                model.Email.Trim());

            // 3. Comprobar que el usuario exista y esté activo.
            if (usuario == null || !usuario.Activo)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "El enlace de recuperación no es válido o ha expirado.");

                model.NewPassword = string.Empty;
                model.ConfirmPassword = string.Empty;

                return View(model);
            }

            // 4. Restablecer la contraseña.
            // Identity valida el token y actualiza el Security Stamp.
            var resultado = await _userManager.ResetPasswordAsync(
                usuario,
                model.Token,
                model.NewPassword);

            // 5. Procesar los errores de Identity.
            if (!resultado.Succeeded)
            {
                foreach (var error in resultado.Errors)
                {
                    if (error.Code == "InvalidToken")
                    {
                        ModelState.AddModelError(
                            string.Empty,
                            "El enlace de restablecimiento no es válido, ha expirado o ya fue utilizado. Solicita uno nuevo.");
                    }
                    else
                    {
                        ModelState.AddModelError(
                            string.Empty,
                            error.Description);
                    }
                }

                // Limpiar las contraseñas antes de devolver la vista.
                model.NewPassword = string.Empty;
                model.ConfirmPassword = string.Empty;

                return View(model);
            }

            // 6. Mostrar la confirmación.
            return RedirectToAction(
                nameof(ResetPasswordConfirmation));
        }

        // GET: /Account/CambiarPassword desde la cuenta del usuario.
        [HttpGet]
        [Authorize]
        public IActionResult CambiarPassword()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarPassword(CambiarPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // Obtener el usuario actual.
            var usuario = await _userManager.GetUserAsync(User);

            if (usuario == null || !usuario.Activo)
            {
                await _signInManager.SignOutAsync();
                return RedirectToAction(nameof(Login));
            }

            var resultado = await _userManager.ChangePasswordAsync(
                usuario,
                model.PasswordActual,
                model.NuevaPassword);

            if (!resultado.Succeeded)
            {
                foreach (var error in resultado.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Code switch
                    {
                        "PasswordMismatch" => "La contraseña actual es incorrecta.",
                        "PasswordTooShort" => "La contraseña debe tener al menos 8 caracteres.",
                        "PasswordRequiresDigit" => "Debe contener al menos un número.",
                        "PasswordRequiresUpper" => "Debe contener al menos una mayúscula.",
                        "PasswordRequiresLower" => "Debe contener al menos una minúscula.",
                        "PasswordRequiresNonAlphanumeric" => "Debe contener al menos un carácter especial.",
                        "PasswordRequiresUniqueChars" => "Debe contener al menos 4 caracteres diferentes.",
                        _ => "No fue posible cambiar la contraseña."
                    });
                }

                return View(model);
            }
            // Actualizar el Security Stamp para forzar la revalidación de la sesión del usuario.

            // Cerrar la sesión después de cambiar la contraseña.
            await _signInManager.SignOutAsync();

            // Mostrar un mensaje de confirmación en Login.
            TempData["Success"] =
                "Tu contraseña se cambió correctamente. Inicia sesión con tu nueva contraseña.";

            // Redirigir al inicio de sesión.
            return RedirectToAction(nameof(Login));

        }

    }
}
