using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGAT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SGAT.Infrastructure.Data.Configurations
{
    public class EquipoConfiguration : IEntityTypeConfiguration<Equipo>
    {
        public void Configure(EntityTypeBuilder<Equipo> builder)
        {
            builder.HasKey(e => e.IdEquipo);

            builder.Property(e => e.IdUsuario).HasMaxLength(450).IsRequired();
            builder.Property(e => e.Modelo).HasMaxLength(100).IsRequired();
            builder.Property(e => e.Serial).HasMaxLength(100);
            builder.HasIndex(e => e.Serial).IsUnique().HasFilter("[Serial] IS NOT NULL");
            builder.Property(e => e.Descripcion).HasMaxLength(255);

            builder.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
            builder.Property(e => e.UpdatedAt).HasDefaultValueSql("GETDATE()");

            builder.HasOne(e => e.Usuario)
                .WithMany(u => u.Equipos)
                .HasForeignKey(e => e.IdUsuario)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(e => e.Cliente)
                .WithMany(c => c.Equipos)
                .HasForeignKey(e => e.IdCliente)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(e => e.CategoriaDispositivo)
                .WithMany(c => c.Equipos)
                .HasForeignKey(e => e.IdCategoriaDispositivo)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(e => e.Marca)
                .WithMany(m => m.Equipos)
                .HasForeignKey(e => e.IdMarca)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
