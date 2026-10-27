using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGAT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SGAT.Infrastructure.Data.Configurations
{
    public class FacturaDetalleConfiguration : IEntityTypeConfiguration<FacturaDetalle>
    {
        public void Configure(EntityTypeBuilder<FacturaDetalle> builder)
        {
            builder.HasKey(d => d.IdFacturaDetalle);

            builder.Property(d => d.Tipo).HasConversion<string>().HasMaxLength(20).IsRequired();
            builder.Property(d => d.Descripcion).HasMaxLength(255).IsRequired();
            builder.Property(d => d.Cantidad).HasColumnType("decimal(10,2)").HasDefaultValue(1);
            builder.Property(d => d.PrecioUnitario).HasColumnType("decimal(10,2)");

            builder.Property(d => d.Subtotal)
                .HasColumnType("decimal(10,2)")
                .HasComputedColumnSql("[Cantidad] * [PrecioUnitario]", stored: true)
                .ValueGeneratedOnAddOrUpdate();

            builder.HasOne(d => d.Factura)
                .WithMany(f => f.Detalles)
                .HasForeignKey(d => d.IdFactura)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(d => d.Producto)
                .WithMany(p => p.FacturaDetalles)
                .HasForeignKey(d => d.IdProducto)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(d => d.LoteProducto)
                .WithMany(l => l.FacturaDetalles)
                .HasForeignKey(d => d.IdLoteProducto)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(d => d.OrdenServicioDetalle)
                .WithMany(o => o.FacturaDetalles)
                .HasForeignKey(d => d.IdOrdenServicioDetalle)
                .OnDelete(DeleteBehavior.NoAction);

            builder.ToTable(t => t.HasCheckConstraint(
                "CK_FacturaDetalle_Tipo",
                "[Tipo] IN ('PRODUCTO','SERVICIO')"));
        }
    }

}
