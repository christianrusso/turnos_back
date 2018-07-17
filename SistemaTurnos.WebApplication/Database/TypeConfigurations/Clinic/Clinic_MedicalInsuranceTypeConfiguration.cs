using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaTurnos.WebApplication.Database.ClinicModel;

namespace SistemaTurnos.WebApplication.Database.TypeConfigurations.Clinic
{
    public class Clinic_MedicalInsuranceTypeConfiguration : IEntityTypeConfiguration<Clinic_MedicalInsurance>
    {
        public void Configure(EntityTypeBuilder<Clinic_MedicalInsurance> builder)
        {
            // Medical Insurance Data
            builder
                .HasOne(mi => mi.Data)
                .WithMany()
                .HasForeignKey(mi => mi.DataId)
                .HasConstraintName("FK_MedicalInsurance_Data");

            // User
            builder
                .HasOne(mi => mi.User)
                .WithMany()
                .HasForeignKey(mi => mi.UserId)
                .HasConstraintName("FK_MedicalInsurance_User");
        }
    }
}
