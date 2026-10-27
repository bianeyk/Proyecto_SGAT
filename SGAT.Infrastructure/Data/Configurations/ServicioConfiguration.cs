using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGAT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SGAT.Infrastructure.Data.Configurations
{
    public class ServicioConfiguration : IEntityTypeConfiguration<Servicio>
    {
        public void Configure(EntityTypeBuilder<Servicio> builder)
        {
            builder.HasKey(s => s.IdServicio);

            builder.Property(s => s.Nombre).HasMaxLength(100).IsRequired();
            builder.Property(s => s.DescripcionBase).HasMaxLength(255);
            builder.Property(s => s.PrecioBase).HasColumnType("decimal(10,2)");
            builder.Property(s => s.Activo).HasDefaultValue(true);
        }
    }
}
