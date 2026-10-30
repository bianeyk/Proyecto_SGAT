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
            builder.Property(c => c.NumDoc).HasMaxLength(50).IsRequired();
            builder.HasIndex(c => c.NumDoc).IsUnique();
            builder.Property(c => c.Nombre).HasMaxLength(250).IsRequired();
            builder.Property(c => c.Telefono).HasMaxLength(20).IsRequired();
            builder.Property(c => c.Email).HasMaxLength(150);
            builder.Property(c => c.Direccion).HasMaxLength(255);
            builder.Property(c => c.EsColaborador).HasDefaultValue(false);
            builder.Property(c => c.Activo).HasDefaultValue(true);
            builder.Property(c => c.CreatedAt).HasDefaultValueSql("GETDATE()");
            builder.Property(c => c.UpdatedAt).HasDefaultValueSql("GETDATE()");

            builder.HasOne(c => c.UsuarioCrea)
                .WithMany(u => u.Clientes)
                .HasForeignKey(c => c.IdUsuarioCrea)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
