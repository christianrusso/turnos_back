using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaTurnos.Database.ClinicModel;

namespace SistemaTurnos.Database.TypeConfigurations.Clinic
{
    public class Clinic_AppointmentTypeConfiguration : IEntityTypeConfiguration<Clinic_Appointment>
    {
        public void Configure(EntityTypeBuilder<Clinic_Appointment> builder)
        {
            // Doctor
            builder
                .HasOne(a => a.Doctor)
                .WithMany(d => d.Appointments)
                .HasForeignKey(a => a.DoctorId)
                .IsRequired()
                .HasConstraintName("FK_Appointment_Doctor");

            // Rating
            builder
                .HasOne(a => a.Rating)
                .WithOne(r => r.Appointment)
                .HasForeignKey<Clinic_Rating>(r => r.AppointmentId)
                .HasConstraintName("FK_Appointment_Rating");

            // Patient
            builder
                .HasOne(a => a.Patient)
                .WithMany(p => p.Appointments)
                .HasForeignKey(a => a.PatientId)
                .HasConstraintName("FK_Appointment_Patient");

            // User
            builder
                .HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .HasConstraintName("FK_Appointment_User");
        }
    }
}
