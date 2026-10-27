using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGAT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SGAT.Infrastructure.Data.Configurations
{
    public class HistorialServicioConfiguration : IEntityTypeConfiguration<HistorialServicio>
    {
        public void Configure(EntityTypeBuilder<HistorialServicio> builder)
        {
            builder.HasKey(h => h.IdHistorialServicio);

            builder.Property(h => h.IdUsuario).HasMaxLength(450).IsRequired();
            builder.Property(h => h.Comentario).HasColumnType("varchar(max)");
            builder.Property(h => h.CreatedAt).HasDefaultValueSql("GETDATE()");

            builder.HasOne(h => h.OrdenServicio)
                .WithMany(o => o.HistorialesServicio)
                .HasForeignKey(h => h.IdOrdenServicio)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(h => h.EstadoServicio)
                .WithMany(e => e.HistorialesServicio)
                .HasForeignKey(h => h.IdEstadoServicio)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(h => h.Usuario)
                .WithMany(u => u.HistorialesServicio)
                .HasForeignKey(h => h.IdUsuario)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
