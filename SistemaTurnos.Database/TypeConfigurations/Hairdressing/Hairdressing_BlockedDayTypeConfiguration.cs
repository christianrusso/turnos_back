using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaTurnos.Database.HairdressingModel;

namespace SistemaTurnos.Database.TypeConfigurations.Hairdressing
{
    public class Hairdressing_BlockedDayTypeConfiguration : IEntityTypeConfiguration<Hairdressing_BlockedDay>
    {
        public void Configure(EntityTypeBuilder<Hairdressing_BlockedDay> builder)
        {
            // Subspecialty
            builder
                .HasOne(bd => bd.Subspecialty)
                .WithMany()
                .HasForeignKey(bd => bd.SubspecialtyId)
                .HasConstraintName("FK_Hairdressing_BlockedDay_Subspecialty");

            // Professional
            builder
                .HasOne(bd => bd.Professional)
                .WithMany(d => d.BlockedDays)
                .HasForeignKey(bd => bd.ProfessionalId)
                .HasConstraintName("FK_Hairdressing_BlockedDay_Professional");
        }
    }
}
