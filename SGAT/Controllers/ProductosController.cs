using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SGAT.Application.DTOs.Producto;
using SGAT.Application.Interfaces.Services;
using SGAT.SGAT.Domain.Enums;
using SGAT.Web.Security;
using SGAT.Web.ViewModels.Producto;

namespace SGAT.Web.Controllers
{
    [Authorize(Roles = "Administrador,Gerente")]
    public class ProductosController : Controller
    {
        private readonly IProductoService _productoService;

        private const int TamanoPagina = 25;
        private const int MaxPagina = 10_000;
        private const int MinLongitudBusqueda = 2;
        private const int MaxLongitudBusqueda = 100;

        public ProductosController(IProductoService productoService)
        {
            _productoService = productoService;
        }

        // GET: /Productos
        [HttpGet]
        [EnableRateLimiting(RateLimitPolicies.Search)]
        public async Task<IActionResult> Index(string? termino, string filtro = "todos", int pagina = 1)
        {
            // Se limita la página para evitar desplazamientos inválidos o costosos.
            pagina = Math.Clamp(pagina, 1, MaxPagina);
            termino = termino?.Trim();
            if (!string.IsNullOrWhiteSpace(termino)
                && termino.Length < MinLongitudBusqueda)
            {
                return BadRequest("El término de búsqueda debe tener al menos 2 caracteres.");
            }

            if (termino?.Length > MaxLongitudBusqueda)
                return BadRequest("El término de búsqueda no puede superar los 100 caracteres.");

            filtro = string.IsNullOrWhiteSpace(filtro)
                ? "todos"
                : filtro.Trim().ToLowerInvariant();
            var result = await _productoService.GetPageAsync(
                string.IsNullOrWhiteSpace(termino) ? null : termino,
                filtro,
                pagina,
                TamanoPagina);

            var vm = new ProductoIndexViewModel
            {
                Productos = result.Items,
                Termino = termino,
                Filtro = filtro,
                PaginaActual = pagina,
                TotalRegistros = result.TotalCount,
                TotalActivos = result.ActiveCount,
                TotalInactivos = result.InactiveCount,
                TotalStockBajo = result.LowStockCount,
                TotalPaginas = Math.Max(1,
                    (int)Math.Ceiling(result.TotalCount / (double)TamanoPagina))
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
