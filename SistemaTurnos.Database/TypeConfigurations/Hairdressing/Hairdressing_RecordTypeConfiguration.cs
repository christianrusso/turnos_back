using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaTurnos.Database.HairdressingModel;

namespace SistemaTurnos.Database.TypeConfigurations.Hairdressing
{
    public class Hairdressing_RecordTypeConfiguration : IEntityTypeConfiguration<Hairdressing_Record>
    {
        public void Configure(EntityTypeBuilder<Hairdressing_Record> builder)
        {
            // Patient
            builder
                .HasOne(r => r.Patient)
                .WithMany(p => p.Records)
                .HasForeignKey(r => r.PatientId)
                .HasConstraintName("FK_Hairdressing_Record_Patient");
        }
    }
}
