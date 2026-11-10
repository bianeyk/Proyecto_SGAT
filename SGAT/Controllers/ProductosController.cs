using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGAT.Application.DTOs.Producto;
using SGAT.Application.Interfaces.Services;
using SGAT.SGAT.Domain.Enums;
using SGAT.Web.ViewModels.Producto;

namespace SGAT.Web.Controllers
{
    [Authorize(Roles = "Administrador,Gerente")]
    public class ProductosController : Controller
    {
        private readonly IProductoService _productoService;

        public ProductosController(IProductoService productoService)
        {
            _productoService = productoService;
        }

        // GET: /Productos
        [HttpGet]
        public async Task<IActionResult> Index(string? termino, string filtro = "todos")
        {
            var productos = await _productoService.GetTodosAsync();

            if (!string.IsNullOrWhiteSpace(termino))
            {
                var texto = termino.Trim();
                productos = productos.Where(p =>
                    p.Nombre.Contains(texto, StringComparison.OrdinalIgnoreCase) ||
                    p.Codigo.Contains(texto, StringComparison.OrdinalIgnoreCase) ||
                    p.Categoria.Contains(texto, StringComparison.OrdinalIgnoreCase) ||
                    p.Marca.Contains(texto, StringComparison.OrdinalIgnoreCase));
            }

            productos = (filtro ?? "todos").ToLowerInvariant() switch
            {
                "activos" => productos.Where(p => p.Activo),
                "inactivos" => productos.Where(p => !p.Activo),
                "stockbajo" => productos.Where(p => p.StockBajo),
                _ => productos
            };

            var vm = new ProductoIndexViewModel
            {
                Productos = productos.OrderBy(p => p.Nombre).ToList(),
                Termino = termino,
                Filtro = string.IsNullOrWhiteSpace(filtro) ? "todos" : filtro.ToLowerInvariant()
            };

            return View(vm);
        }

        // GET: /Productos/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
                return NotFound();

            var producto = await _productoService.GetByIdAsync(id);
            if (producto is null)
                return NotFound();

            return View(ToDetalle(producto));
        }

        // GET: /Productos/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var vm = new ProductoViewModel();
            await CargarListasAsync(vm);
            return View(vm);
        }

        // POST: /Productos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductoViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                await CargarListasAsync(vm);
                return View(vm);
            }

            try
            {
                var creado = await _productoService.CrearAsync(ToDto(vm));

                TempData["Success"] = "Producto creado correctamente.";
                return RedirectToAction(nameof(Details), new { id = creado.IdProducto });
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(nameof(vm.Codigo), ex.Message);
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            await CargarListasAsync(vm);
            return View(vm);
        }

        // GET: /Productos/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0)
                return NotFound();

            var producto = await _productoService.GetByIdAsync(id);
            if (producto is null)
                return NotFound();

            var vm = new ProductoViewModel
            {
                IdProducto = producto.IdProducto,
                IdCategoriaProducto = producto.IdCategoriaProducto,
                IdMarca = producto.IdMarca,
                Codigo = producto.Codigo,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                Condicion = Enum.TryParse<CondicionProductoEnum>(producto.Condicion, ignoreCase: true, out var condicion)
                    ? condicion
                    : CondicionProductoEnum.NUEVO,
                PrecioVenta = producto.PrecioVenta,
                PrecioDescuento = producto.PrecioDescuento,
                StockMinimo = producto.StockMinimo,
                Activo = producto.Activo
            };

            await CargarListasAsync(vm);
            return View(vm);
        }

        // POST: /Productos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductoViewModel vm)
        {
            if (id != vm.IdProducto)
                return NotFound();

            var productoActual = await _productoService.GetByIdAsync(id);
            if (productoActual is null)
                return NotFound();

            if (!ModelState.IsValid)
            {
                await CargarListasAsync(vm);
                return View(vm);
            }

            try
            {
                await _productoService.ActualizarAsync(id, ToDto(vm));

                TempData["Success"] = "Producto actualizado correctamente.";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(nameof(vm.Codigo), ex.Message);
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            await CargarListasAsync(vm);
            return View(vm);
        }

        // POST: /Productos/ActivarDesactivar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivarDesactivar(int id)
        {
            try
            {
                await _productoService.ActivarDesactivarAsync(id);
                TempData["Success"] = "Estado del producto actualizado correctamente.";
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }

        // ── Helpers ──────────────────────────────────────────────

        private async Task CargarListasAsync(ProductoViewModel vm)
        {
            var categorias = await _productoService.GetCategoriasAsync();
            vm.Categorias = categorias
                .Select(c => new CategoriaProductoOptionViewModel
                {
                    IdCategoriaProducto = c.IdCategoriaProducto,
                    Nombre = c.Nombre
                })
                .ToList();

            var marcas = await _productoService.GetMarcasAsync();
            vm.Marcas = marcas
                .Select(m => new MarcaOptionViewModel
                {
                    IdMarca = m.IdMarca,
                    Nombre = m.Nombre
                })
                .ToList();
        }

        private static CrearProductoDto ToDto(ProductoViewModel vm) => new()
        {
            IdCategoriaProducto = vm.IdCategoriaProducto,
            IdMarca = vm.IdMarca,
            Codigo = string.IsNullOrWhiteSpace(vm.Codigo) ? null : vm.Codigo.Trim(),
            Nombre = vm.Nombre.Trim(),
            Descripcion = string.IsNullOrWhiteSpace(vm.Descripcion) ? null : vm.Descripcion.Trim(),
            Condicion = vm.Condicion.ToString(),
            PrecioVenta = vm.PrecioVenta,
            PrecioDescuento = vm.PrecioDescuento,
            StockMinimo = vm.StockMinimo,
            Activo = vm.Activo
        };

        private static ProductoDetalleViewModel ToDetalle(ProductoDto p) => new()
        {
            IdProducto = p.IdProducto,
            Codigo = p.Codigo,
            Nombre = p.Nombre,
            Descripcion = p.Descripcion,
            Categoria = p.Categoria,
            Marca = p.Marca,
            Condicion = p.Condicion,
            PrecioVenta = p.PrecioVenta,
            PrecioDescuento = p.PrecioDescuento,
            StockActual = p.StockActual,
            StockMinimo = p.StockMinimo,
            StockReservado = p.StockReservado,
            Activo = p.Activo,
            CreatedAt = p.CreatedAt
        };
    }
}
