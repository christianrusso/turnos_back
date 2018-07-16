using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaTurnos.WebApplication.Database.ClinicModel;

namespace SistemaTurnos.WebApplication.Database.TypeConfigurations.Clinic
{
    public class Clinic_SubspecialtyTypeConfiguration : IEntityTypeConfiguration<Clinic_Subspecialty>
    {
        public void Configure(EntityTypeBuilder<Clinic_Subspecialty> builder)
        {
            // Specialty
            builder
                .HasOne(ssp => ssp.Specialty)
                .WithMany(s => s.Subspecialties)
                .HasForeignKey(ssp => ssp.SpecialtyId)
                .IsRequired()
                .HasConstraintName("FK_SubSpecialty_Specialty");

            // User
            builder
                .HasOne(ss => ss.User)
                .WithMany()
                .HasForeignKey(ss => ss.UserId)
                .HasConstraintName("FK_Subspecialty_User");
        }
    }
}
