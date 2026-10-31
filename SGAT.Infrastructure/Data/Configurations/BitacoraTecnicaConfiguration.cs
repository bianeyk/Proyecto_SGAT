using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGAT.Domain.Entities;

namespace SGAT.Infrastructure.Data.Configurations
{
    public class BitacoraTecnicaConfiguration : IEntityTypeConfiguration<BitacoraTecnica>
    {
        public void Configure(EntityTypeBuilder<BitacoraTecnica> builder)
        {
            builder.HasKey(b => b.Id);

            builder.Property(b => b.Nota)
                .HasMaxLength(1000)
                .IsRequired();

            builder.Property(b => b.FechaRegistro)
                .HasDefaultValueSql("GETUTCDATE()");

            // Relación con DeleteBehavior.Restrict
            builder.HasOne(b => b.Orden)
                .WithMany(o => o.Bitacoras)
                .HasForeignKey(b => b.OrdenId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
