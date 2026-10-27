using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGAT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SGAT.Infrastructure.Data.Configurations
{
    public class OrdenServicioDetalleConfiguration : IEntityTypeConfiguration<OrdenServicioDetalle>
    {
        public void Configure(EntityTypeBuilder<OrdenServicioDetalle> builder)
        {
            builder.HasKey(d => d.IdOrdenServicioDetalle);
            //Agergar propiedad de IdusuarioCrea
            builder.Property(o => o.IdUsuarioCrea).HasMaxLength(450).IsRequired();
            builder.Property(d => d.Descripcion).HasMaxLength(255).IsRequired();
            builder.Property(d => d.Observacion).HasColumnType("varchar(max)");
            builder.Property(d => d.CreatedAt).HasDefaultValueSql("GETDATE()");

            builder.HasOne(d => d.OrdenServicio)
                .WithMany(o => o.Detalles)
                .HasForeignKey(d => d.IdOrdenServicio)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(d => d.Servicio)
                .WithMany(s => s.OrdenesServicioDetalle)
                .HasForeignKey(d => d.IdServicio)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
