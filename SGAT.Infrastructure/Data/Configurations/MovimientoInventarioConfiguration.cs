using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGAT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SGAT.Infrastructure.Data.Configurations
{
    public class MovimientoInventarioConfiguration : IEntityTypeConfiguration<MovimientoInventario>
    {
        public void Configure(EntityTypeBuilder<MovimientoInventario> builder)
        {
            builder.HasKey(m => m.IdMovimientoInventario);

            builder.Property(m => m.Tipo).HasConversion<string>().HasMaxLength(20).IsRequired();
            builder.Property(m => m.Cantidad).HasColumnType("decimal(10,2)");
            builder.Property(m => m.StockAnterior).HasColumnType("decimal(10,2)");
            builder.Property(m => m.StockNuevo).HasColumnType("decimal(10,2)");
            builder.Property(m => m.Observacion).HasMaxLength(255);
            builder.Property(m => m.CreatedAt).HasDefaultValueSql("GETDATE()");

            builder.HasOne(m => m.Producto)
                .WithMany(p => p.Movimientos)
                .HasForeignKey(m => m.IdProducto)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(m => m.Proveedor)
                .WithMany(p => p.Movimientos)
                .HasForeignKey(m => m.IdProveedor)
                .OnDelete(DeleteBehavior.NoAction);

            builder.ToTable(t => t.HasCheckConstraint(
                "CK_MovimientoInventario_Tipo",
                "[Tipo] IN ('COMPRA','VENTA','RESERVA','LIBERACION_RESERVA')"));
        }
    }
}
