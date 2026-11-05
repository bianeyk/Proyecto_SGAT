using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SGAT.Domain.Entities;

namespace SGAT.Infrastructure.Data
{
    public class ApplicationDbContext: IdentityDbContext<ApplicationUser,ApplicationRole,string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext>options):base (options)
        {
            
        }

        // --- Negocio ---
        public DbSet<Auditoria> Auditorias => Set<Auditoria>();
        public DbSet<BitacoraTecnica> BitacorasTecnicas => Set<BitacoraTecnica>();
        public DbSet<CategoriaDispositivo> CategoriasDispositivo => Set<CategoriaDispositivo>();
        public DbSet<CategoriaProducto> CategoriasProducto => Set<CategoriaProducto>();
        public DbSet<Cliente> Clientes => Set<Cliente>();
        public DbSet<Compra> Compras => Set<Compra>();
        public DbSet<CompraDetalle> CompraDetalles => Set<CompraDetalle>();
        public DbSet<DiagnosticoTecnico> DiagnosticosTecnicos => Set<DiagnosticoTecnico>();
        public DbSet<Empresa> Empresas => Set<Empresa>();
        public DbSet<Equipo> Equipos => Set<Equipo>();
        public DbSet<EstadoServicio> EstadosServicio => Set<EstadoServicio>();
        public DbSet<Factura> Facturas => Set<Factura>();
        public DbSet<FacturaDetalle> FacturaDetalles => Set<FacturaDetalle>();
        public DbSet<GarantiaCliente> GarantiasCliente => Set<GarantiaCliente>();
        public DbSet<GarantiaProveedor> GarantiasProveedor => Set<GarantiaProveedor>();
        public DbSet<HistorialServicio> HistorialServicios => Set<HistorialServicio>();
        public DbSet<LoteProducto> LotesProducto => Set<LoteProducto>();
        public DbSet<Marca> Marcas => Set<Marca>();
        public DbSet<MovimientoInventario> MovimientosInventario => Set<MovimientoInventario>();
        public DbSet<OrdenServicio> OrdenesServicio => Set<OrdenServicio>();
        public DbSet<OrdenServicioDetalle> OrdenServicioDetalles => Set<OrdenServicioDetalle>();
        public DbSet<OrdenServicioDetalleProducto> OrdenServicioDetalleProductos => Set<OrdenServicioDetalleProducto>();
        public DbSet<PagoCartera> PagosCartera => Set<PagoCartera>();
        public DbSet<Permiso> Permisos => Set<Permiso>();
        public DbSet<Producto> Productos => Set<Producto>();
        public DbSet<Proveedor> Proveedores => Set<Proveedor>();
        public DbSet<RolePermiso> RolePermisos => Set<RolePermiso>();
        public DbSet<Servicio> Servicios => Set<Servicio>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); // aplica el modelo de Identity primero

            // Aplica TODAS las IEntityTypeConfiguration<> del assembly (Identity + negocio)
            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }

    }
}
