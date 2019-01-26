using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaTurnos.Database.ClinicModel;

namespace SistemaTurnos.Database.TypeConfigurations.Clinic
{
    public class Clinic_BlockedDayTypeConfiguration : IEntityTypeConfiguration<Clinic_BlockedDay>
    {
        public void Configure(EntityTypeBuilder<Clinic_BlockedDay> builder)
        {
            // Subspecialty
            builder
                .HasOne(bd => bd.Subspecialty)
                .WithMany()
                .HasForeignKey(bd => bd.SubspecialtyId)
                .HasConstraintName("FK_BlockedDay_Subspecialty");

            // Doctor
            builder
                .HasOne(bd => bd.Doctor)
                .WithMany(d => d.BlockedDays)
                .HasForeignKey(bd => bd.DoctorId)
                .HasConstraintName("FK_BlockedDay_Doctor");
        }
    }
}
