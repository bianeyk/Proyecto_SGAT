using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGAT.Domain.Entities;
using SGAT.SGAT.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SGAT.Infrastructure.Data.Configurations
{
    public class OrdenServicioConfiguration : IEntityTypeConfiguration<OrdenServicio>
    {
        public void Configure(EntityTypeBuilder<OrdenServicio> builder)
        {
            builder.HasKey(o => o.IdOrdenServicio);

            builder.Property(o => o.IdUsuarioCrea).HasMaxLength(450).IsRequired();
            builder.Property(o => o.Codigo).HasMaxLength(100).IsRequired();
            builder.HasIndex(o => o.Codigo).IsUnique();
            builder.Property(o => o.Diagnostico).HasColumnType("varchar(max)");
            builder.Property(o => o.CostoTotal).HasColumnType("decimal(10,2)").HasDefaultValue(0);
            builder.Property(o => o.Abono).HasColumnType("decimal(10,2)").HasDefaultValue(0);
            builder.Property(o => o.Prioridad)
                .HasConversion<string>()
                .HasMaxLength(10)
                .HasDefaultValue(PrioridadEnum.MEDIA);

            builder.Property(o => o.FechaIngreso).HasDefaultValueSql("GETDATE()");
            builder.Property(o => o.Observaciones).HasColumnType("varchar(max)");

            builder.HasOne(o => o.Equipo)
                .WithMany(e => e.OrdenesServicio)
                .HasForeignKey(o => o.IdEquipo)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(o => o.UsuarioCrea)
                .WithMany(u => u.OrdenesCreadas)
                .HasForeignKey(o => o.IdUsuarioCrea)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(o => o.EstadoServicio)
                .WithMany(e => e.OrdenesServicio)
                .HasForeignKey(o => o.IdEstadoServicio)
                .OnDelete(DeleteBehavior.NoAction);

            builder.ToTable(t => t.HasCheckConstraint(
                "CK_OrdenServicio_Prioridad",
                "[Prioridad] IN ('BAJA','MEDIA','ALTA')"));
        }
    }
}
