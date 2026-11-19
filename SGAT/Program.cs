using System.Globalization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SGAT.Domain.Entities;
using SGAT.Infrastructure.Data;
using SGAT.Infrastructure.Seed;
using SGAT.Application.DependencyInjection;
using SGAT.Infrastructure.DependencyInjection;



var builder = WebApplication.CreateBuilder(args);

// Cultura de FORMATO: punto (.) como separador decimal, sin importar la
// configuración regional del sistema. Evita el error de parseo en [Range]
// (por ejemplo "0.01") y en el binding de decimales desde <input type="number">.
// La cultura de INTERFAZ se deja en el idioma del sistema (español).
CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("es-MX");

// 1. REGISTRAR CAPA DE INFRAESTRUCTURA / PERSISTENCIA (Repositorios y DB)
builder.Services.AddInfrastructure(builder.Configuration); // El nombre puede variar según tu proyecto

// 2. REGISTRAR CAPA DE APLICACIÓN (Tus servicios)
builder.Services.AddApplication();

//Registro de base de datos y servicios
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration
               .GetConnectionString("DefaultConnection"));
});
//	Registrar el Servicio de Identity (Dependency Injection)
builder.Services
    .AddIdentity<ApplicationUser, ApplicationRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

//configurar la ruta de login
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";

    options.AccessDeniedPath =
        "/Account/AccessDenied";
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

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Redirección a HTTPS también en desarrollo. El perfil "https" de
// launchSettings.json expone https://localhost:7137 y http://localhost:5224.
// Para que el navegador no marque el sitio como inseguro, hay que confiar el
// certificado de desarrollo en el almacén del sistema (ver README de desarrollo).
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
//AUTENTICACION
app.UseAuthentication();
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

    // 3. Ejecutamos el administrador (Ahora sí se creará)
    await SeedRoles.SeedAdminAsync(userManager);
}


//Ejucuta
app.Run();

