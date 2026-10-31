using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGAT.Domain.Entities;

namespace SGAT.Infrastructure.Data.Configurations
{
    public class DiagnosticoTecnicoConfiguration : IEntityTypeConfiguration<DiagnosticoTecnico>
    {
        public void Configure(EntityTypeBuilder<DiagnosticoTecnico> builder)
        {
            builder.HasKey(d => d.Id);

            builder.Property(d => d.FallaReportada)
                .HasMaxLength(500);

            builder.Property(d => d.DiagnosticoDetallado)
                .HasMaxLength(2000)
                .IsRequired();

            builder.Property(d => d.SolucionAplicada)
                .HasMaxLength(2000);

            builder.Property(d => d.RepuestosSugeridos)
                .HasMaxLength(1000);

            builder.Property(d => d.CostoManoDeObra)
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0);

            builder.Property(d => d.CostoTotalEstimado)
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0);

            builder.Property(d => d.FechaInicio)
                .HasDefaultValueSql("GETUTCDATE()");

            // Relaciones con DeleteBehavior.Restrict
            builder.HasOne(d => d.Orden)
                .WithOne(o => o.DiagnosticoTecnico)
                .HasForeignKey<DiagnosticoTecnico>(d => d.OrdenId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(d => d.Tecnico)
                .WithMany(u => u.Diagnosticos)
                .HasForeignKey(d => d.TecnicoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
