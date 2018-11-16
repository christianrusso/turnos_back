using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaTurnos.Database.ClinicModel;

namespace SistemaTurnos.Database.TypeConfigurations.Clinic
{
    public class Clinic_DoctorTypeConfiguration : IEntityTypeConfiguration<Clinic_Doctor>
    {
        public void Configure(EntityTypeBuilder<Clinic_Doctor> builder)
        {
            // Specialty
            builder
                .HasOne(d => d.Specialty)
                .WithMany(s => s.Doctors)
                .HasForeignKey(d => d.SpecialtyId)
                .IsRequired()
                .HasConstraintName("FK_Doctor_Specialty");

            // Subspecialty
            builder
                .HasOne(d => d.Subspecialty)
                .WithMany()
                .HasForeignKey(d => d.SubspecialtyId)
                .HasConstraintName("FK_Doctor_Subspecialty");

            // User
            builder
                .HasOne(d => d.User)
                .WithMany()
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Doctor_User");
        }
    }
}
