using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaTurnos.WebApplication.Database.ClinicModel;

namespace SistemaTurnos.WebApplication.Database.TypeConfigurations.Clinic
{
    public class Clinic_SpecialtyTypeConfiguration : IEntityTypeConfiguration<Clinic_Specialty>
    {
        public void Configure(EntityTypeBuilder<Clinic_Specialty> builder)
        {
            // Specialty Data
            builder
                .HasOne(s => s.Data)
                .WithMany()
                .HasForeignKey(s => s.DataId)
                .HasConstraintName("FK_Specialty_Data");

            // User
            builder
                .HasOne(s => s.User)
                .WithMany()
                .HasForeignKey(s => s.UserId)
                .HasConstraintName("FK_Specialty_User");
        }
    }
}
