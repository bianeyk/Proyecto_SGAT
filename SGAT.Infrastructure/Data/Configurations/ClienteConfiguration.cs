using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGAT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SGAT.Infrastructure.Data.Configurations
{
    public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
    {
        public void Configure(EntityTypeBuilder<Cliente> builder)
        {
            builder.HasKey(c => c.IdCliente);

            builder.Property(c => c.IdUsuarioCrea).HasMaxLength(450).IsRequired();
            builder.Property(c => c.NumDoc).HasMaxLength(15).IsRequired();
            builder.HasIndex(c => c.NumDoc).IsUnique();
            builder.Property(c => c.Nombre).HasMaxLength(250).IsRequired();
            builder.HasIndex(c => c.Nombre);
            builder.Property(c => c.Telefono).HasMaxLength(20).IsRequired();
            builder.Property(c => c.Email).HasMaxLength(150);
            builder.HasIndex(c => c.Email);
            builder.Property(c => c.Direccion).HasMaxLength(255);
            builder.HasIndex(c => c.Telefono);
            builder.Property(c => c.EsColaborador).HasDefaultValue(false);
            builder.Property(c => c.Activo).HasDefaultValue(true);
            builder.Property(c => c.CreatedAt).HasPrecision(0)
              .HasDefaultValueSql("CAST(SYSDATETIMEOFFSET() AT TIME ZONE 'SA Pacific Standard Time' AS DATETIME2(0))");
            builder.Property(c => c.UpdatedAt).HasPrecision(0)
              .HasDefaultValueSql("CAST(SYSDATETIMEOFFSET() AT TIME ZONE 'SA Pacific Standard Time' AS DATETIME2(0))");

            builder.HasOne(c => c.UsuarioCrea)
                .WithMany(u => u.Clientes)
                .HasForeignKey(c => c.IdUsuarioCrea)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
