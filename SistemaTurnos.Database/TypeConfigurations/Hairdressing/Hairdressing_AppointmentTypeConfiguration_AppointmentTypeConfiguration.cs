using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaTurnos.Database.HairdressingModel;

namespace SistemaTurnos.Database.TypeConfigurations.Hairdressing
{
    public class Hairdressing_AppointmentTypeConfiguration : IEntityTypeConfiguration<Hairdressing_Appointment>
    {
        public void Configure(EntityTypeBuilder<Hairdressing_Appointment> builder)
        {
            // Professional
            builder
                .HasOne(a => a.Professional)
                .WithMany(d => d.Appointments)
                .HasForeignKey(a => a.ProfessionalId)
                .IsRequired()
                .HasConstraintName("FK_Hairdressing_Appointment_Professional");

            // Rating
            builder
                .HasOne(a => a.Rating)
                .WithOne(r => r.Appointment)
                .HasForeignKey<Hairdressing_Rating>(r => r.AppointmentId)
                .HasConstraintName("FK_Hairdressing_Appointment_Rating");

            // Patient
            builder
                .HasOne(a => a.Patient)
                .WithMany(p => p.Appointments)
                .HasForeignKey(a => a.PatientId)
                .HasConstraintName("FK_Hairdressing_Appointment_Patient");

            // User
            builder
                .HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .HasConstraintName("FK_Hairdressing_Appointment_User");
        }
    }
}
