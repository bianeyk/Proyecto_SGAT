using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SGAT.Domain.Entities;
using SGAT.Infrastructure.Data;
using SGAT.Infrastructure.Seed;
using SGAT.Application.DependencyInjection;
using SGAT.Infrastructure.DependencyInjection;



var builder = WebApplication.CreateBuilder(args);

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
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
//AUTENTICACION
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

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

