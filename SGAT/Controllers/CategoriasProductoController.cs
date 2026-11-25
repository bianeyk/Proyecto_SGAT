using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGAT.Application.DTOs.Producto;
using SGAT.Application.Interfaces.Services;
using SGAT.Web.ViewModels.CategoriaProducto;

namespace SGAT.Web.Controllers
{
    [Authorize(Roles = "Administrador,Gerente")]
    public class CategoriasProductoController : Controller
    {
        private readonly ICategoriaProductoService _categoriaService;

        private const int TamanoPagina = 25;
        private const int MaxPagina = 100_000;

        public CategoriasProductoController(ICategoriaProductoService categoriaService)
        {
            _categoriaService = categoriaService;
        }

        // GET: /CategoriasProducto
        [HttpGet]
        public async Task<IActionResult> Index(string? termino, int pagina = 1)
        {
            // Se controla el valor recibido desde la URL antes de consultar datos.
            pagina = Math.Clamp(pagina, 1, MaxPagina);
            var result = await _categoriaService.GetPageAsync(
                string.IsNullOrWhiteSpace(termino) ? null : termino.Trim(),
                pagina,
                TamanoPagina);

            var vm = new CategoriaProductoIndexViewModel
            {
                Categorias = result.Items,
                Termino = termino,
                PaginaActual = pagina,
                TotalRegistros = result.TotalCount,
                TotalPaginas = Math.Max(1,
                    (int)Math.Ceiling(result.TotalCount / (double)TamanoPagina))
            };

            return View(vm);
        }

        // GET: /CategoriasProducto/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View(new CategoriaProductoViewModel { Activo = true });
        }

        // POST: /CategoriasProducto/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoriaProductoViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            try
            {
                await _categoriaService.CrearAsync(new CategoriaProductoDto
                {
                    Nombre = vm.Nombre,
                    Descripcion = vm.Descripcion,
                    Activo = vm.Activo
                });

                TempData["Success"] = "Categoría creada correctamente.";
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

        // GET: /CategoriasProducto/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0)
                return NotFound();

            var categoria = await _categoriaService.GetByIdAsync(id);
            if (categoria is null)
                return NotFound();

            return View(new CategoriaProductoViewModel
            {
                IdCategoriaProducto = categoria.IdCategoriaProducto,
                Nombre = categoria.Nombre,
                Descripcion = categoria.Descripcion,
                Activo = categoria.Activo
            });
        }

        // POST: /CategoriasProducto/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoriaProductoViewModel vm)
        {
            if (id != vm.IdCategoriaProducto)
                return NotFound();

            if (!ModelState.IsValid)
                return View(vm);

            try
            {
                await _categoriaService.ActualizarAsync(id, new CategoriaProductoDto
                {
                    Nombre = vm.Nombre,
                    Descripcion = vm.Descripcion,
                    Activo = vm.Activo
                });

                TempData["Success"] = "Categoría actualizada correctamente.";
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

        // POST: /CategoriasProducto/ActivarDesactivar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivarDesactivar(int id)
        {
            try
            {
                await _categoriaService.ActivarDesactivarAsync(id);
                TempData["Success"] = "Estado de la categoría actualizado correctamente.";
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
