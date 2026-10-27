using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGAT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SGAT.Infrastructure.Data.Configurations
{
    public class GarantiaProveedorConfiguration : IEntityTypeConfiguration<GarantiaProveedor>
    {
        public void Configure(EntityTypeBuilder<GarantiaProveedor> builder)
        {
            builder.HasKey(g => g.IdGarantiaProveedor);

            builder.Property(g => g.Estado).HasConversion<string>().HasMaxLength(20);
            builder.Property(g => g.Observacion).HasMaxLength(255);

            builder.HasOne(g => g.LoteProducto)
                .WithMany(l => l.GarantiasProveedor)
                .HasForeignKey(g => g.IdLoteProducto)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(g => g.Proveedor)
                .WithMany(p => p.GarantiasProveedor)
                .HasForeignKey(g => g.IdProveedor)
                .OnDelete(DeleteBehavior.NoAction);

            builder.ToTable(t => t.HasCheckConstraint(
                "CK_GarantiaProveedor_Estado",
                "[Estado] IN ('ENVIADO','EN_PROCESO','RESUELTO','RECHAZADO')"));
        }
    }
}
