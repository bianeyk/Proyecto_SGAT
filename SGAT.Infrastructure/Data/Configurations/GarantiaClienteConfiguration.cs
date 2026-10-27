using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGAT.Domain.Entities;
using SGAT.SGAT.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SGAT.Infrastructure.Data.Configurations
{
    public class GarantiaClienteConfiguration : IEntityTypeConfiguration<GarantiaCliente>
    {
        public void Configure(EntityTypeBuilder<GarantiaCliente> builder)
        {
            builder.HasKey(g => g.IdGarantiaCliente);

            builder.Property(g => g.FechaInicio).HasDefaultValueSql("GETDATE()");
            builder.Property(g => g.GarantiaDias).IsRequired();

            builder.Property(g => g.FechaFinGarantia)
                .HasComputedColumnSql("DATEADD(DAY, [GarantiaDias], [FechaInicio])", stored: false)
                .ValueGeneratedOnAddOrUpdate();

            builder.Property(g => g.Estado)
                .HasConversion<string>()
                .HasMaxLength(20)
                .HasDefaultValue(EstadoGarantiaEnum.ACTIVA);
            builder.Property(g => g.Observaciones).HasMaxLength(255);

            builder.HasOne(g => g.FacturaDetalle)
                .WithMany(f => f.GarantiasCliente)
                .HasForeignKey(g => g.IdFacturaDetalle)
                .OnDelete(DeleteBehavior.NoAction);

            builder.ToTable(t => t.HasCheckConstraint(
                "CK_GarantiaCliente_Estado",
                "[Estado] IN ('ACTIVA','VENCIDA','ANULADA','PROCESO')"));
        }
    }
}
