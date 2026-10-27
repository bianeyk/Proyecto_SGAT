using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGAT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SGAT.Infrastructure.Data.Configurations
{
    public class PagoCarteraConfiguration : IEntityTypeConfiguration<PagoCartera>
    {
        public void Configure(EntityTypeBuilder<PagoCartera> builder)
        {
            builder.HasKey(p => p.IdPagoCartera);

            builder.Property(p => p.Abono).HasColumnType("decimal(10,2)").IsRequired();
            builder.Property(p => p.FechaPago).HasDefaultValueSql("GETDATE()");
            builder.Property(p => p.MetodoPago).HasConversion<string>().HasMaxLength(20).IsRequired();
            builder.Property(p => p.Referencia).HasMaxLength(100);
            builder.Property(p => p.Observacion).HasMaxLength(255);
            builder.Property(p => p.CreatedAt).HasDefaultValueSql("GETDATE()");

            builder.HasOne(p => p.Factura)
                .WithMany(f => f.PagosCartera)
                .HasForeignKey(p => p.IdFactura)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(p => p.Cliente)
                .WithMany(c => c.PagosCartera)
                .HasForeignKey(p => p.IdCliente)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(p => p.OrdenServicio)
                .WithMany(o => o.PagosCartera)
                .HasForeignKey(p => p.IdOrdenServicio)
                .OnDelete(DeleteBehavior.NoAction);

            builder.ToTable(t => t.HasCheckConstraint(
                "CK_PagoCartera_MetodoPago",
                "[MetodoPago] IN ('EFECTIVO','TRANSFERENCIA','OTRO')"));
        }
    }
}
