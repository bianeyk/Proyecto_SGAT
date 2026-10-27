using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGAT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SGAT.Infrastructure.Data.Configurations
{
    public class ProductoConfiguration : IEntityTypeConfiguration<Producto>
    {
        public void Configure(EntityTypeBuilder<Producto> builder)
        {
            builder.HasKey(p => p.IdProducto);

            builder.Property(p => p.Codigo).HasMaxLength(100);
            builder.HasIndex(p => p.Codigo).IsUnique().HasFilter("[Codigo] IS NOT NULL");
            builder.Property(p => p.Nombre).HasMaxLength(150).IsRequired();
            builder.Property(p => p.Descripcion).HasColumnType("varchar(max)");
            builder.Property(p => p.Condicion).HasConversion<string>().HasMaxLength(20).IsRequired();

            builder.Property(p => p.PrecioVenta).HasColumnType("decimal(10,2)");
            builder.Property(p => p.PrecioDescuento).HasColumnType("decimal(10,2)");
            builder.Property(p => p.StockActual).HasColumnType("decimal(10,2)").HasDefaultValue(0);
            builder.Property(p => p.StockMinimo).HasColumnType("decimal(10,2)").HasDefaultValue(0);
            builder.Property(p => p.StockReservado).HasColumnType("decimal(10,2)").HasDefaultValue(0);

            builder.Property(p => p.Activo).HasDefaultValue(true);
            builder.Property(p => p.CreatedAt).HasDefaultValueSql("GETDATE()");

            builder.HasOne(p => p.CategoriaProducto)
                .WithMany(c => c.Productos)
                .HasForeignKey(p => p.IdCategoriaProducto)
                .OnDelete(DeleteBehavior.NoAction);

            builder.ToTable(t => t.HasCheckConstraint(
                "CK_Producto_Condicion",
                "[Condicion] IN ('NUEVO','USADO','REMANUFACTURADO')"));
        }
    }
}
