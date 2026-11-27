using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SGAT.Application.DTOs.Equipo;
using SGAT.Application.Interfaces.Services;
using SGAT.Web.ViewModels.Equipo;

namespace SGAT.Web.Controllers;

[Authorize(Roles = "Administrador,Gerente")]
public sealed class CategoriasDispositivoController(
    ICategoriaDispositivoService servicio) : Controller
{
    [HttpGet]
    [EnableRateLimiting("SearchPolicy")]
    public async Task<IActionResult> Index(string? termino, int pagina = 1)
    {
        try
        {
            var resultado = await servicio.GetPageAsync(termino, pagina, 25);
            ViewData["Termino"] = termino;
            ViewData["Pagina"] = Math.Clamp(pagina, 1, 10_000);
            ViewData["TotalPaginas"] = Math.Max(1,
                (int)Math.Ceiling(resultado.TotalCount / 25m));
            return View(resultado);
        }
        catch (ArgumentException ex) { return BadRequest(ex.Message); }
    }

    [HttpGet]
    public IActionResult Create() =>
        View("Formulario", new CategoriaDispositivoViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CategoriaDispositivoViewModel modelo)
    {
        if (!ModelState.IsValid) return View("Formulario", modelo);
        try
        {
            await servicio.CrearAsync(ConstruirDto(modelo));
            TempData["Success"] = "Categoría de dispositivo creada correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        { ModelState.AddModelError(nameof(modelo.Nombre), ex.Message); }
        catch (ArgumentException ex)
        { ModelState.AddModelError(string.Empty, ex.Message); }
        return View("Formulario", modelo);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var categoria = await servicio.GetByIdAsync(id);
        if (categoria is null) return NotFound();
        return View("Formulario", new CategoriaDispositivoViewModel
        {
            IdCategoriaDispositivo = categoria.IdCategoriaDispositivo,
            Nombre = categoria.Nombre, Descripcion = categoria.Descripcion,
            Activo = categoria.Activo
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id, CategoriaDispositivoViewModel modelo)
    {
        if (id != modelo.IdCategoriaDispositivo) return NotFound();
        if (!ModelState.IsValid) return View("Formulario", modelo);
        try
        {
            await servicio.ActualizarAsync(id, ConstruirDto(modelo));
            TempData["Success"] = "Categoría de dispositivo actualizada correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (KeyNotFoundException) { return NotFound(); }
        catch (InvalidOperationException ex)
        { ModelState.AddModelError(nameof(modelo.Nombre), ex.Message); }
        catch (ArgumentException ex)
        { ModelState.AddModelError(string.Empty, ex.Message); }
        return View("Formulario", modelo);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ActivarDesactivar(int id)
    {
        try
        {
            await servicio.ActivarDesactivarAsync(id);
            TempData["Success"] = "Estado de la categoría de dispositivo actualizado.";
        }
        catch (KeyNotFoundException) { return NotFound(); }
        return RedirectToAction(nameof(Index));
    }

    private static CategoriaDispositivoDto ConstruirDto(
        CategoriaDispositivoViewModel modelo) =>
        new(modelo.IdCategoriaDispositivo, modelo.Nombre,
            modelo.Descripcion, modelo.Activo);
}
