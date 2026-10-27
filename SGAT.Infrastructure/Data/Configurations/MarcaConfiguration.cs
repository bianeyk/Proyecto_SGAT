using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGAT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SGAT.Infrastructure.Data.Configurations
{
    public class MarcaConfiguration : IEntityTypeConfiguration<Marca>
    {
        public void Configure(EntityTypeBuilder<Marca> builder)
        {
            builder.HasKey(m => m.IdMarca);

            builder.Property(m => m.Nombre).HasMaxLength(100).IsRequired();
            builder.HasIndex(m => m.Nombre).IsUnique();
            builder.Property(m => m.Descripcion).HasMaxLength(255);
            builder.Property(m => m.Activo).HasDefaultValue(true);
        }
    }
}
