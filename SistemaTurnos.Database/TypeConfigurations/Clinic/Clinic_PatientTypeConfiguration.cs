using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaTurnos.Database.ClinicModel;

namespace SistemaTurnos.Database.TypeConfigurations.Clinic
{
    public class Clinic_PatientTypeConfiguration : IEntityTypeConfiguration<Clinic_Patient>
    {
        public void Configure(EntityTypeBuilder<Clinic_Patient> builder)
        {
            // Medical Plan
            builder
                .HasOne(p => p.MedicalPlan)
                .WithMany(mp => mp.Patients)
                .HasForeignKey(p => p.MedicalPlanId)
                .HasConstraintName("FK_Patient_MedicalPlan");

            // User
            builder
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .HasConstraintName("FK_Patient_User");

            // Client
            builder
                .HasOne(p => p.Client)
                .WithMany(c => c.Patients)
                .HasForeignKey(p => p.ClientId)
                .HasConstraintName("FK_Patient_Client");
        }
    }
}
