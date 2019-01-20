using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaTurnos.Database.ClinicModel;

namespace SistemaTurnos.Database.TypeConfigurations.Clinic
{
    public class Clinic_DoctorSubspecialtyTypeConfiguration : IEntityTypeConfiguration<Clinic_DoctorSubspecialty>
    {
        public void Configure(EntityTypeBuilder<Clinic_DoctorSubspecialty> builder)
        {
            // Subspecialty
            builder
                .HasOne(ds => ds.Subspecialty)
                .WithMany()
                .HasForeignKey(d => d.SubspecialtyId)
                .HasConstraintName("FK_DoctorSubspecialty_Subspecialty");

            // Doctor
            builder
                .HasOne(ds => ds.Doctor)
                .WithMany(d => d.Subspecialties)
                .HasForeignKey(d => d.DoctorId)
                .HasConstraintName("FK_DoctorSubspecialty_Doctor");
        }
    }
}
