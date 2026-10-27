using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGAT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SGAT.Infrastructure.Data.Configurations
{
    public class CategoriaProductoConfiguration : IEntityTypeConfiguration<CategoriaProducto>
    {
        public void Configure(EntityTypeBuilder<CategoriaProducto> builder)
        {
            builder.HasKey(c => c.IdCategoriaProducto);

            builder.Property(c => c.Nombre).HasMaxLength(100).IsRequired();
            builder.HasIndex(c => c.Nombre).IsUnique();
            builder.Property(c => c.Descripcion).HasColumnType("varchar(max)");
            builder.Property(c => c.Activo).HasDefaultValue(true);
        }
    }
}
