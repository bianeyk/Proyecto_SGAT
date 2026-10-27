using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGAT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SGAT.Infrastructure.Data.Configurations
{
    public class RolePermisoConfiguration : IEntityTypeConfiguration<RolePermiso>
    {
        public void Configure(EntityTypeBuilder<RolePermiso> builder)
        {
            builder.HasKey(rp => new { rp.RoleId, rp.IdPermiso });

            builder.Property(rp => rp.RoleId).HasMaxLength(450);
            builder.Property(rp => rp.CreatedAt).HasDefaultValueSql("GETDATE()");

            builder.HasOne(rp => rp.Role)
                .WithMany(r => r.RolesPermisos)
                .HasForeignKey(rp => rp.RoleId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(rp => rp.Permiso)
                .WithMany(p => p.RolesPermisos)
                .HasForeignKey(rp => rp.IdPermiso)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
