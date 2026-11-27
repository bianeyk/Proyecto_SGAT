using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SGAT.Application.DTOs.Equipo;
using SGAT.Application.Interfaces.Services;
using SGAT.Web.ViewModels.Equipo;

namespace SGAT.Web.Controllers;

[Authorize(Roles = "Administrador,Gerente,Tecnico")]
public sealed class EquiposController(IEquipoService equipos, IClienteService clientes) : Controller
{
    [HttpGet]
    [EnableRateLimiting("SearchPolicy")]
    public async Task<IActionResult> Index(string? termino, string estado = "todos", int pagina = 1)
    {
        pagina = Math.Clamp(pagina, 1, 1_000);
        try
        {
            var resultado = await equipos.GetPageAsync(termino, estado, pagina, 25);
            ViewData["Termino"] = termino;
            ViewData["Estado"] = estado;
            ViewData["Pagina"] = pagina;
            ViewData["TotalPaginas"] = Math.Max(1, (int)Math.Ceiling(resultado.TotalCount / 25m));
            return View(resultado);
        }
        catch (ArgumentException ex) { return BadRequest(ex.Message); }
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var equipo = await equipos.GetByIdAsync(id);
        return equipo is null ? NotFound() : View(equipo);
    }

    [HttpGet]
    [EnableRateLimiting("SearchPolicy")]
    public async Task<IActionResult> BuscarClientes(string termino)
    {
        try { return Json(await equipos.BuscarClientesAsync(termino)); }
        catch (ArgumentException ex) { return BadRequest(ex.Message); }
    }

    [HttpGet]
    public async Task<IActionResult> Create(int? idCliente)
    {
        var vm = new EquipoViewModel { IdCliente = idCliente ?? 0 };
        await CargarOpcionesAsync(vm);
        return View("Formulario", vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EquipoViewModel vm)
    {
        var usuario = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(usuario)) return Forbid();
        if (ModelState.IsValid)
        {
            try
            {
                var id = await equipos.CrearAsync(ToDto(vm), usuario);
                TempData["Success"] = "Equipo registrado correctamente.";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
            { ModelState.AddModelError(string.Empty, ex.Message); }
        }
        vm.IdEquipo = 0;
        await CargarOpcionesAsync(vm);
        return View("Formulario", vm);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var equipo = await equipos.GetByIdAsync(id);
        if (equipo is null) return NotFound();
        var vm = new EquipoViewModel
        {
            IdEquipo = id, IdCliente = equipo.IdCliente, IdMarca = equipo.IdMarca,
            IdCategoriaDispositivo = equipo.IdCategoriaDispositivo,
            Modelo = equipo.Modelo, Serial = equipo.Serial, Descripcion = equipo.Descripcion,
            SerialGenerado = equipo.SerialGenerado,
            Version = equipo.Version
        };
        await CargarOpcionesAsync(vm, equipo);
        return View("Formulario", vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EquipoViewModel vm)
    {
        if (id != vm.IdEquipo) return NotFound();
        var actual = await equipos.GetByIdAsync(id);
        if (actual is null) return NotFound();
        if (ModelState.IsValid)
        {
            try
            {
                await equipos.ActualizarAsync(id, ToDto(vm));
                TempData["Success"] = "Equipo actualizado correctamente.";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (KeyNotFoundException) { return NotFound(); }
            catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
            { ModelState.AddModelError(string.Empty, ex.Message); }
        }
        await CargarOpcionesAsync(vm, actual);
        return View("Formulario", vm);
    }

    [HttpPost]
    [Authorize(Roles = "Administrador,Gerente")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CambiarEstado(int id, bool activo, string version)
    {
        try
        {
            await equipos.CambiarEstadoAsync(id, activo, version);
            TempData["Success"] = activo ? "Equipo activado." : "Equipo dado de baja. Su historial se conserva.";
        }
        catch (KeyNotFoundException) { return NotFound(); }
        catch (InvalidOperationException ex) { TempData["Error"] = ex.Message; }
        return RedirectToAction(nameof(Details), new { id });
    }

    private async Task CargarOpcionesAsync(EquipoViewModel vm, EquipoDto? actual = null)
    {
        vm.TieneOrdenes = actual?.TieneOrdenes ?? false;
        // Solo se muestran catálogos activos y el valor histórico realmente guardado.
        vm.Marcas = await equipos.GetMarcasAsync(actual?.IdMarca);
        vm.Categorias = await equipos.GetCategoriasAsync(actual?.IdCategoriaDispositivo);
        if (vm.TieneOrdenes && actual is not null)
        {
            vm.IdCliente = actual.IdCliente;
            ModelState.Remove(nameof(vm.IdCliente));
        }
        if (vm.IdCliente > 0)
        {
            var cliente = await clientes.GetByIdAsync(vm.IdCliente);
            if (cliente is not null && (cliente.Activo || cliente.IdCliente == actual?.IdCliente))
                vm.Clientes = [new(cliente.IdCliente, $"{cliente.Nombre} — {cliente.NumDocumento}")];
        }
    }

    private static GuardarEquipoDto ToDto(EquipoViewModel vm) => new()
    {
        IdCliente = vm.IdCliente, IdMarca = vm.IdMarca, IdCategoriaDispositivo = vm.IdCategoriaDispositivo,
        Modelo = vm.Modelo, Serial = vm.Serial, SerialGenerado = vm.SerialGenerado,
        Descripcion = vm.Descripcion, Version = vm.Version
    };
}
