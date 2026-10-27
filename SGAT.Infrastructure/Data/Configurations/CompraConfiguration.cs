using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGAT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SGAT.Infrastructure.Data.Configurations
{
    public class CompraConfiguration : IEntityTypeConfiguration<Compra>
    {
        public void Configure(EntityTypeBuilder<Compra> builder)
        {
            builder.HasKey(c => c.IdCompra);

            builder.Property(c => c.IdUsuario).HasMaxLength(450).IsRequired();
            builder.Property(c => c.NumFactura).HasMaxLength(100);
            builder.Property(c => c.FechaCompra).HasDefaultValueSql("GETDATE()");
            builder.Property(c => c.Total).HasColumnType("decimal(10,2)").HasDefaultValue(0);
            builder.Property(c => c.Observaciones).HasColumnType("varchar(max)");

            builder.HasOne(c => c.Proveedor)
                .WithMany(p => p.Compras)
                .HasForeignKey(c => c.IdProveedor)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(c => c.Usuario)
                .WithMany(u => u.Compras)
                .HasForeignKey(c => c.IdUsuario)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
