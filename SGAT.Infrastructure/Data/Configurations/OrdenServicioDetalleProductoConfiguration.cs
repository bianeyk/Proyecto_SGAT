using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGAT.Domain.Entities;
using SGAT.SGAT.Domain.Enums;

namespace SGAT.Infrastructure.Data.Configurations
{
    public class OrdenServicioDetalleProductoConfiguration : IEntityTypeConfiguration<OrdenServicioDetalleProducto>
    {
        public void Configure(EntityTypeBuilder<OrdenServicioDetalleProducto> builder)
        {
            builder.HasKey(d => d.IdOrdenServicioDetalleProducto);

            builder.Property(d => d.Cantidad).HasColumnType("decimal(10,2)");
            builder.Property(d => d.PrecioUnitario).HasColumnType("decimal(10,2)");
            builder.Property(d => d.Estado)
                .HasConversion<string>()
                .HasMaxLength(20)
                .HasDefaultValue(EstadoOrdenServicioProductoEnum.RESERVADO);

            builder.Property(d => d.Subtotal)
                .HasColumnType("decimal(10,2)")
                .HasComputedColumnSql("[Cantidad] * [PrecioUnitario]", stored: true)
                .ValueGeneratedOnAddOrUpdate();

            builder.HasOne(d => d.LoteProducto)
                .WithMany(l => l.DetallesOrdenServicio)
                .HasForeignKey(d => d.IdLoteProducto)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(d => d.OrdenServicio)
                .WithMany(o => o.DetallesProducto)
                .HasForeignKey(d => d.IdOrdenServicio)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(d => d.Producto)
                .WithMany(p => p.DetallesOrdenServicio)
                .HasForeignKey(d => d.IdProducto)
                .OnDelete(DeleteBehavior.NoAction);

            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_OrdenServicioDetalleProducto_Cantidad", "[Cantidad] > 0");
                t.HasCheckConstraint(
                    "CK_OrdenServicioDetalleProducto_Estado",
                    "[Estado] IN ('RESERVADO','ENTREGADO','CANCELADO','DEVUELTO')");
            });
        }
    }
}
