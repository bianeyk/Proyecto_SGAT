using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGAT.Domain.Entities;
using SGAT.SGAT.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SGAT.Infrastructure.Data.Configurations
{
    public class FacturaConfiguration : IEntityTypeConfiguration<Factura>
    {
        public void Configure(EntityTypeBuilder<Factura> builder)
        {
            builder.HasKey(f => f.IdFactura);

            builder.Property(f => f.IdUsuario).HasMaxLength(450).IsRequired();
            builder.Property(f => f.NumeroFactura).HasMaxLength(50).IsRequired();
            builder.HasIndex(f => f.NumeroFactura).IsUnique();
            builder.Property(f => f.FechaFactura).HasDefaultValueSql("GETDATE()");

            builder.Property(f => f.Subtotal).HasColumnType("decimal(10,2)").HasDefaultValue(0);
            builder.Property(f => f.Iva).HasColumnType("decimal(10,2)").HasDefaultValue(0);
            builder.Property(f => f.Total).HasColumnType("decimal(10,2)").HasDefaultValue(0);
            builder.Property(f => f.SaldoPendiente).HasColumnType("decimal(10,2)").HasDefaultValue(0);

            builder.Property(f => f.MetodoPago).HasConversion<string>().HasMaxLength(20).IsRequired();
            builder.Property(f => f.Estado)
                .HasConversion<string>()
                .HasMaxLength(20)
                .HasDefaultValue(EstadoFacturaEnum.PENDIENTE);
            builder.Property(f => f.Observaciones).HasColumnType("varchar(max)");
            builder.Property(f => f.GarantiaDias).HasDefaultValue(0);
            builder.Property(f => f.CreatedAt).HasDefaultValueSql("GETDATE()");

            builder.HasOne(f => f.Cliente)
                .WithMany(c => c.Facturas)
                .HasForeignKey(f => f.IdCliente)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(f => f.OrdenServicio)
                .WithMany(o => o.Facturas)
                .HasForeignKey(f => f.IdOrdenServicio)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(f => f.Usuario)
                .WithMany(u => u.Facturas)
                .HasForeignKey(f => f.IdUsuario)
                .OnDelete(DeleteBehavior.NoAction);

            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_Factura_Estado", "[Estado] IN ('PENDIENTE','PAGADA','ANULADA')");
                t.HasCheckConstraint(
                    "CK_Factura_MetodoPago",
                    "[MetodoPago] IN ('EFECTIVO','TRANSFERENCIA','TARJETA','OTRO')");
            });
        }
    }
}
