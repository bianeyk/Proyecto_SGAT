using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGAT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SGAT.Infrastructure.Data.Configurations
{
    public class PermisoConfiguration : IEntityTypeConfiguration<Permiso>
    {
        public void Configure(EntityTypeBuilder<Permiso> builder)
        {
            builder.HasKey(p => p.IdPermiso);

            builder.Property(p => p.Codigo).HasMaxLength(100).IsRequired();
            builder.HasIndex(p => p.Codigo).IsUnique();
            builder.Property(p => p.Descripcion).HasMaxLength(255);
        }
    }
}
