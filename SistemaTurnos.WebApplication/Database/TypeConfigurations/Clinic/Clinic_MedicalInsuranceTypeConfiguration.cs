using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaTurnos.WebApplication.Database.ClinicModel;

namespace SistemaTurnos.WebApplication.Database.TypeConfigurations.Clinic
{
    public class Clinic_MedicalInsuranceTypeConfiguration : IEntityTypeConfiguration<Clinic_MedicalInsurance>
    {
        public void Configure(EntityTypeBuilder<Clinic_MedicalInsurance> builder)
        {
            // User
            builder
                .HasOne(s => s.User)
                .WithMany()
                .HasForeignKey(s => s.UserId)
                .HasConstraintName("FK_MedicalInsurance_User");
        }
    }
}
