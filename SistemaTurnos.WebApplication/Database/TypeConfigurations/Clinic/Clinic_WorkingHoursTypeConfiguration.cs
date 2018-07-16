using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaTurnos.WebApplication.Database.ClinicModel;

namespace SistemaTurnos.WebApplication.Database.TypeConfigurations.Clinic
{
    public class Clinic_WorkingHoursTypeConfiguration : IEntityTypeConfiguration<Clinic_WorkingHours>
    {
        public void Configure(EntityTypeBuilder<Clinic_WorkingHours> builder)
        {
            // Doctor
            builder
                .HasOne(wh => wh.Doctor)
                .WithMany(d => d.WorkingHours)
                .HasForeignKey(wh => wh.DoctorId)
                .IsRequired()
                .HasConstraintName("FK_WorkingHours_Doctor");
        }
    }
}
