using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGAT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SGAT.Infrastructure.Data.Configurations
{
    public class EmpresaConfiguration : IEntityTypeConfiguration<Empresa>
    {
        public void Configure(EntityTypeBuilder<Empresa> builder)
        {
            builder.HasKey(e => e.IdEmpresa);

            builder.Property(e => e.Nombre).HasMaxLength(150).IsRequired();
            builder.Property(e => e.Nit).HasMaxLength(50).IsRequired();
            builder.HasIndex(e => e.Nit).IsUnique();
            builder.Property(e => e.Direccion).HasMaxLength(255);
            builder.Property(e => e.Telefono).HasMaxLength(20);
            builder.Property(e => e.Email).HasMaxLength(150);
            builder.Property(e => e.Politicas).HasColumnType("varchar(max)");

            builder.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
        }
    }
}
