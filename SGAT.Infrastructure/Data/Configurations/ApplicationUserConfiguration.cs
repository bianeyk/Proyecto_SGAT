using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGAT.Domain.Entities;

namespace SGAT.Infrastructure.Data.Configurations
{
    public class ApplicationUserConfiguration
        : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(
            EntityTypeBuilder<ApplicationUser> builder)
        {
            // Configuración del nombre.
            builder.Property(u => u.NombreCompleto)
                .HasMaxLength(100)
                .IsRequired();
            // Correo normalizado único.
            builder.HasIndex(u => u.NormalizedEmail)
                .IsUnique()
                .HasDatabaseName("EmailIndex")
                .HasFilter("[NormalizedEmail] IS NOT NULL");
            // Índice de búsqueda por nombre.
            builder.HasIndex(u => u.NombreCompleto)
                .HasDatabaseName("IX_AspNetUsers_NombreCompleto");


        }
    }
}