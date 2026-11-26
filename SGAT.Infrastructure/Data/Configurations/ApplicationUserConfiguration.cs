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
            builder.Property(u => u.NombreCompleto)
                .HasMaxLength(100)
                .IsRequired();

            builder.HasIndex(u => u.NormalizedEmail)
                .IsUnique()
                .HasDatabaseName("EmailIndex")
                .HasFilter("[NormalizedEmail] IS NOT NULL");
        }
    }
}