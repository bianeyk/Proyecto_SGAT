using System.Globalization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SGAT.Domain.Entities;
using SGAT.Infrastructure.Data;
using SGAT.Infrastructure.Seed;
using SGAT.Application.DependencyInjection;
using SGAT.Infrastructure.DependencyInjection;
using SGAT.Web.Middleware;
using SGAT.Infrastructure.Identity;
using System.Threading.RateLimiting;



var currentDir = Directory.GetCurrentDirectory();
var contentRoot = currentDir;

if (!Directory.Exists(Path.Combine(currentDir, "wwwroot")))
{
    var baseDir = AppContext.BaseDirectory;
    var candidate = Path.GetFullPath(Path.Combine(baseDir, "..", "..", ".."));
    if (Directory.Exists(Path.Combine(candidate, "wwwroot")))
    {
        contentRoot = candidate;
    }
    else if (Directory.Exists(Path.Combine(baseDir, "wwwroot")))
    {
        contentRoot = baseDir;
    }
}

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = contentRoot,
    WebRootPath = "wwwroot"
});

// Cultura de FORMATO: punto (.) como separador decimal, sin importar la
// configuración regional del sistema. Evita el error de parseo en [Range]
// (por ejemplo "0.01") y en el binding de decimales desde <input type="number">.
// La cultura de INTERFAZ se deja en el idioma del sistema (español).
CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("es-MX");

// 1. REGISTRAR CAPA DE INFRAESTRUCTURA / PERSISTENCIA (Repositorios y DB)
builder.Services.AddInfrastructure(builder.Configuration); // El nombre puede variar según tu proyecto

// 2. REGISTRAR CAPA DE APLICACIÓN (Tus servicios)
builder.Services.AddApplication();


//	Registrar el Servicio de Identity (Dependency Injection)
builder.Services
    .AddIdentity<ApplicationUser, ApplicationRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddErrorDescriber<SpanishIdentityErrorDescriber>();
//determina cuánto tiempo será válido un token antes de expirar
builder.Services.Configure<DataProtectionTokenProviderOptions>(
    options =>
    {
        options.TokenLifespan = TimeSpan.FromHours(1);
    });

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode =
        StatusCodes.Status429TooManyRequests;
    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.StatusCode =
            StatusCodes.Status429TooManyRequests;

        context.HttpContext.Response.ContentType =
            "text/plain; charset=utf-8";

        await context.HttpContext.Response.WriteAsync(
            "Demasiados intentos. Espera unos minutos antes de volver a intentarlo.",
            cancellationToken);
    };
    options.AddPolicy("LoginPolicy", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey:
                context.Connection.RemoteIpAddress?.ToString()
                ?? "unknown",

            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0,
                AutoReplenishment = true
            }));
    options.AddPolicy("ForgotPasswordPolicy", context =>
    RateLimitPartition.GetFixedWindowLimiter(
        partitionKey:
            context.Connection.RemoteIpAddress?.ToString()
            ?? "unknown",
        factory: _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 3,
            Window = TimeSpan.FromMinutes(10),
            QueueLimit = 0,
            AutoReplenishment = true
        }));
});

builder.Services.Configure<IdentityOptions>(options =>
{
    // Política de contraseñas
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredUniqueChars = 4;

    // Bloqueo por intentos fallidos
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.AllowedForNewUsers = true;

    // Usuarios únicos
    options.User.RequireUniqueEmail = true;
});

// Configurar la validación del Security Stamp  autenticada procesada por la cookie correspondiente.
builder.Services.Configure<SecurityStampValidatorOptions>(options =>
{
    options.ValidationInterval = TimeSpan.Zero;
});

//configurar la ruta de login. opciones de seguridad y duración de la cookie.
builder.Services.ConfigureApplicationCookie(options =>
{
    // Rutas de autenticación
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";

    // Seguridad de la cookie
    //Impide que JavaScript acceda directamente a la cookie.
    options.Cookie.HttpOnly = true;
    //Solo envía la cookie mediante HTTPS.
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    //Ayuda a reducir ataques CSRF mediante solicitudes entre sitios.
    options.Cookie.SameSite = SameSiteMode.Lax;

    // Duración de la sesión
    //	Establece una duración de 30 minutos para el ticket de autenticación.
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    //Renueva la expiración cuando el usuario sigue utilizando la aplicación.
    options.SlidingExpiration = true;
});

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    // El runtime de .NET no incluye los mensajes del framework en español,
    // así que los definimos explícitamente (errores de binding, números, etc.).
    var mensajes = options.ModelBindingMessageProvider;

    mensajes.SetAttemptedValueIsInvalidAccessor(
        (valor, campo) => string.IsNullOrEmpty(valor)
            ? $"El campo {campo} es obligatorio."
            : $"El valor '{valor}' no es válido para {campo}.");
    mensajes.SetNonPropertyAttemptedValueIsInvalidAccessor(
        valor => $"El valor '{valor}' no es válido.");
    mensajes.SetUnknownValueIsInvalidAccessor(
        campo => $"El valor indicado no es válido para {campo}.");
    mensajes.SetNonPropertyUnknownValueIsInvalidAccessor(
        () => "El valor indicado no es válido.");
    mensajes.SetValueIsInvalidAccessor(
        valor => $"El valor '{valor}' no es válido.");
    mensajes.SetValueMustNotBeNullAccessor(
        campo => $"El campo {campo} es obligatorio.");
    mensajes.SetValueMustBeANumberAccessor(
        campo => $"El campo {campo} debe ser un número.");
    mensajes.SetNonPropertyValueMustBeANumberAccessor(
        () => "El campo debe ser un número.");
    mensajes.SetMissingBindRequiredValueAccessor(
        campo => $"Falta un valor para {campo}.");
    mensajes.SetMissingKeyOrValueAccessor(
        () => "Se requieren la clave y el valor.");
    mensajes.SetMissingRequestBodyRequiredValueAccessor(
        () => "Se requiere el cuerpo de la solicitud.");
});

var app = builder.Build();
//redirige las solicitudes HTTP hacia HTTPS
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios
    app.UseHsts();
}

// Redirección a HTTPS también en desarrollo. El perfil "https" de
// launchSettings.json expone https://localhost:7137 y http://localhost:5224.
// Para que el navegador no marque el sitio como inseguro, hay que confiar el
// certificado de desarrollo en el almacén del sistema (ver README de desarrollo).
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseRateLimiter();

// Autenticación
app.UseAuthentication();

// Comprobar que el usuario y su rol sigan activos.
app.UseMiddleware<ValidarEstadoUsuarioMiddleware>();

// Autorización
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}")
    .WithStaticAssets();

app.MapControllers();

////Using para registro de roles
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    // 1. Obtenemos los dos managers necesarios
    var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

    // 2. Ejecutamos primero los roles (Obligatorio)
    await SeedRoles.SeedAsync(roleManager);

    // 3. Ejecutamos el administrador inicial
    var adminEmail = builder.Configuration[
     "SGAT:Admin:Email"];

    var adminPassword = builder.Configuration[
        "SGAT:Admin:Password"];

    await SeedRoles.SeedAdminAsync(
    userManager,
    roleManager,
    adminEmail,
    adminPassword);
}


//Ejucuta
app.Run();

