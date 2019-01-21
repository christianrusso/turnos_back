using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaTurnos.Database.ClinicModel;

namespace SistemaTurnos.Database.TypeConfigurations.Clinic
{
    public class Clinic_DoctorTypeConfiguration : IEntityTypeConfiguration<Clinic_Doctor>
    {
        public void Configure(EntityTypeBuilder<Clinic_Doctor> builder)
        {
            // User
            builder
                .HasOne(d => d.User)
                .WithMany()
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Doctor_User");
        }
    }
}
