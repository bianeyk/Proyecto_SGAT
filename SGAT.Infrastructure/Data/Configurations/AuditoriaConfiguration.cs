using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGAT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SGAT.Infrastructure.Data.Configurations
{
    public class AuditoriaConfiguration : IEntityTypeConfiguration<Auditoria>
    {
        public void Configure(EntityTypeBuilder<Auditoria> builder)
        {
            builder.HasKey(a => a.IdAuditoria);

            builder.Property(a => a.IdUsuario).HasMaxLength(450).IsRequired();
            builder.Property(a => a.Tabla).HasMaxLength(100).IsRequired();
            builder.Property(a => a.Accion).HasConversion<string>().HasMaxLength(20).IsRequired();
            builder.Property(a => a.ValoresAnteriores).HasColumnType("varchar(max)");
            builder.Property(a => a.ValoresNuevos).HasColumnType("varchar(max)");
            builder.Property(a => a.Modulo).HasMaxLength(100);
            builder.Property(a => a.Observacion).HasMaxLength(255);
            builder.Property(a => a.IpAddress).HasMaxLength(50);
            builder.Property(a => a.CreatedAt).HasDefaultValueSql("GETDATE()");

            builder.HasOne(a => a.Usuario)
                .WithMany(u => u.Auditorias)
                .HasForeignKey(a => a.IdUsuario)
                .OnDelete(DeleteBehavior.NoAction);

            builder.ToTable(t => t.HasCheckConstraint(
                "CK_Auditoria_Accion",
                "[Accion] IN ('INSERT','UPDATE','DELETE','LOGIN','LOGOUT')"));
        }
    }
}


