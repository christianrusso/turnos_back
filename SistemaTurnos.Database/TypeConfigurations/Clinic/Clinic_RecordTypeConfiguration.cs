using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaTurnos.Database.ClinicModel;

namespace SistemaTurnos.Database.TypeConfigurations.Clinic
{
    public class Clinic_RecordTypeConfiguration : IEntityTypeConfiguration<Clinic_Record>
    {
        public void Configure(EntityTypeBuilder<Clinic_Record> builder)
        {
            // Patient
            builder
                .HasOne(r => r.Patient)
                .WithMany(p => p.MedicalRecords)
                .HasForeignKey(r => r.PatientId)
                .HasConstraintName("FK_Record_Patient");
        }
    }
}
