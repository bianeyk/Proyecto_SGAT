using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGAT.Application.DTOs.Producto;
using SGAT.Application.Interfaces.Services;
using SGAT.Web.ViewModels.Marca;

namespace SGAT.Web.Controllers
{
    [Authorize(Roles = "Administrador,Gerente")]
    public class MarcasController : Controller
    {
        private readonly IMarcaService _marcaService;

        private const int TamanoPagina = 25;

        public MarcasController(IMarcaService marcaService)
        {
            _marcaService = marcaService;
        }

        // GET: /Marcas
        [HttpGet]
        public async Task<IActionResult> Index(string? termino, int pagina = 1)
        {
            if (pagina < 1) pagina = 1;

            var marcas = string.IsNullOrWhiteSpace(termino)
                ? await _marcaService.GetTodasAsync()
                : await _marcaService.BuscarAsync(termino.Trim());

            var listaOrdenada = marcas.OrderBy(m => m.Nombre).ToList();
            var total = listaOrdenada.Count;

            var vm = new MarcaIndexViewModel
            {
                Marcas = listaOrdenada
                    .Skip((pagina - 1) * TamanoPagina)
                    .Take(TamanoPagina)
                    .ToList(),
                Termino = termino,
                PaginaActual = pagina,
                TotalPaginas = (int)Math.Ceiling(total / (double)TamanoPagina)
            };

            return View(vm);
        }

        // GET: /Marcas/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View(new MarcaViewModel { Activo = true });
        }

        // POST: /Marcas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MarcaViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            try
            {
                await _marcaService.CrearAsync(new MarcaDto
                {
                    Nombre = vm.Nombre,
                    Descripcion = vm.Descripcion,
                    Activo = vm.Activo
                });

                TempData["Success"] = "Marca creada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(nameof(vm.Nombre), ex.Message);
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            return View(vm);
        }

        // GET: /Marcas/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0)
                return NotFound();

            var marca = await _marcaService.GetByIdAsync(id);
            if (marca is null)
                return NotFound();

            return View(new MarcaViewModel
            {
                IdMarca = marca.IdMarca,
                Nombre = marca.Nombre,
                Descripcion = marca.Descripcion,
                Activo = marca.Activo
            });
        }

        // POST: /Marcas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MarcaViewModel vm)
        {
            if (id != vm.IdMarca)
                return NotFound();

            if (!ModelState.IsValid)
                return View(vm);

            try
            {
                await _marcaService.ActualizarAsync(id, new MarcaDto
                {
                    Nombre = vm.Nombre,
                    Descripcion = vm.Descripcion,
                    Activo = vm.Activo
                });

                TempData["Success"] = "Marca actualizada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(nameof(vm.Nombre), ex.Message);
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            return View(vm);
        }

        // POST: /Marcas/ActivarDesactivar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivarDesactivar(int id)
        {
            try
            {
                await _marcaService.ActivarDesactivarAsync(id);
                TempData["Success"] = "Estado de la marca actualizado correctamente.";
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
