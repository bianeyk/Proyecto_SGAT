using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGAT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SGAT.Infrastructure.Data.Configurations
{
    public class ProveedorConfiguration : IEntityTypeConfiguration<Proveedor>
    {
        public void Configure(EntityTypeBuilder<Proveedor> builder)
        {
            builder.HasKey(p => p.IdProveedor);

            builder.Property(p => p.Nombre).HasMaxLength(150).IsRequired();
            builder.Property(p => p.Nit).HasMaxLength(50).IsRequired();
            builder.HasIndex(p => p.Nit).IsUnique();
            builder.Property(p => p.Contacto).HasMaxLength(150);
            builder.Property(p => p.Telefono).HasMaxLength(20);
            builder.Property(p => p.Email).HasMaxLength(150);
            builder.Property(p => p.Direccion).HasMaxLength(255);

            builder.Property(p => p.Activo).HasDefaultValue(true);
            builder.Property(p => p.CreatedAt).HasDefaultValueSql("GETDATE()");
        }
    }
}
