using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGAT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SGAT.Infrastructure.Data.Configurations
{
    public class CategoriaDispositivoConfiguration : IEntityTypeConfiguration<CategoriaDispositivo>
    {
        public void Configure(EntityTypeBuilder<CategoriaDispositivo> builder)
        {
            builder.HasKey(c => c.IdCategoriaDispositivo);

            builder.Property(c => c.Nombre).HasMaxLength(100).IsRequired();
            builder.HasIndex(c => c.Nombre).IsUnique();
            builder.Property(c => c.Descripcion).HasColumnType("varchar(max)");
            builder.Property(c => c.Activo).HasDefaultValue(true);
        }
    }
}
