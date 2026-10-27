using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGAT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SGAT.Infrastructure.Data.Configurations
{
    public class CompraDetalleConfiguration : IEntityTypeConfiguration<CompraDetalle>
    {
        public void Configure(EntityTypeBuilder<CompraDetalle> builder)
        {
            builder.HasKey(d => d.IdCompraDetalle);

            builder.Property(d => d.Cantidad).HasColumnType("decimal(10,2)");
            builder.Property(d => d.CostoUnitario).HasColumnType("decimal(10,2)");

            builder.Property(d => d.Subtotal)
                .HasColumnType("decimal(10,2)")
                .HasComputedColumnSql("[Cantidad] * [CostoUnitario]", stored: true)
                .ValueGeneratedOnAddOrUpdate();

            builder.HasOne(d => d.Compra)
                .WithMany(c => c.Detalles)
                .HasForeignKey(d => d.IdCompra)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(d => d.Producto)
                .WithMany(p => p.CompraDetalles)
                .HasForeignKey(d => d.IdProducto)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
