using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGAT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SGAT.Infrastructure.Data.Configurations
{
    public class LoteProductoConfiguration : IEntityTypeConfiguration<LoteProducto>
    {
        public void Configure(EntityTypeBuilder<LoteProducto> builder)
        {
            builder.HasKey(l => l.IdLoteProducto);

            builder.Property(l => l.NumeroLote).HasMaxLength(100).IsRequired();
            builder.Property(l => l.Serial).HasMaxLength(150);
            builder.Property(l => l.CantidadInicial).HasColumnType("decimal(10,2)");
            builder.Property(l => l.CantidadDisponible).HasColumnType("decimal(10,2)");
            builder.Property(l => l.CostoUnitario).HasColumnType("decimal(10,2)");
            builder.Property(l => l.FechaIngreso).HasDefaultValueSql("GETDATE()");
            builder.Property(l => l.GarantiaProveedorDias).HasDefaultValue(0);
            builder.Property(l => l.Observaciones).HasMaxLength(255);

            builder.Property(l => l.FechaFinGarantiaProveedor)
                .HasComputedColumnSql("DATEADD(DAY, [GarantiaProveedorDias], [FechaIngreso])", stored: false)
                .ValueGeneratedOnAddOrUpdate();

            builder.HasOne(l => l.CompraDetalle)
                .WithMany(c => c.Lotes)
                .HasForeignKey(l => l.IdCompraDetalle)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(l => l.Producto)
                .WithMany(p => p.Lotes)
                .HasForeignKey(l => l.IdProducto)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
