using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaTurnos.Database.HairdressingModel;

namespace SistemaTurnos.Database.TypeConfigurations.Hairdressing
{
    public class Hairdressing_ProfessionalSubspecialtyTypeConfiguration : IEntityTypeConfiguration<Hairdressing_ProfessionalSubspecialty>
    {
        public void Configure(EntityTypeBuilder<Hairdressing_ProfessionalSubspecialty> builder)
        {
            // Subspecialty
            builder
                .HasOne(ds => ds.Subspecialty)
                .WithMany()
                .HasForeignKey(d => d.SubspecialtyId)
                .HasConstraintName("FK_ProfessionalSubspecialty_Subspecialty");

            // Doctor
            builder
                .HasOne(ds => ds.Professional)
                .WithMany(p => p.Subspecialties)
                .HasForeignKey(d => d.ProfessionalId)
                .HasConstraintName("FK_ProfessionalSubspecialty_Professional");
        }
    }
}
