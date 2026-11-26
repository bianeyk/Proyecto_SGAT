using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGAT.Domain.Entities;

namespace SGAT.Infrastructure.Data.Configurations
{
    public class ApplicationRoleConfiguration
        : IEntityTypeConfiguration<ApplicationRole>
    {
        public void Configure(
            EntityTypeBuilder<ApplicationRole> builder)
        {
            builder.Property(r => r.Name)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(r => r.NormalizedName)
                .HasMaxLength(256)
                .IsRequired();
        }
    }
}
