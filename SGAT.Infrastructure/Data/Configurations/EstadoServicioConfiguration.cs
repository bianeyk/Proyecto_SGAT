using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGAT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SGAT.Infrastructure.Data.Configurations
{
    public class EstadoServicioConfiguration : IEntityTypeConfiguration<EstadoServicio>
    {
        public void Configure(EntityTypeBuilder<EstadoServicio> builder)
        {
            builder.HasKey(e => e.IdEstadoServicio);

            builder.Property(e => e.Nombre).HasMaxLength(50).IsRequired();
            builder.HasIndex(e => e.Nombre).IsUnique();
            builder.Property(e => e.Descripcion).HasMaxLength(255);

            builder.ToTable(t => t.HasCheckConstraint(
                "CK_EstadoServicio_Nombre",
                "[Nombre] IN ('RECIBIDO','DIAGNOSTICO','EN_REPARACION','LISTO','ENTREGADO','CANCELADO','BODEGA')"));
        }
    }
}
