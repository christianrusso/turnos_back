using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaTurnos.WebApplication.Database.ClinicModel;

namespace SistemaTurnos.WebApplication.Database.TypeConfigurations.Clinic
{
    public class Clinic_OpenCloseHoursTypeConfiguration : IEntityTypeConfiguration<Clinic_OpenCloseHours>
    {
        public void Configure(EntityTypeBuilder<Clinic_OpenCloseHours> builder)
        {
            // Clinic
            builder
                .HasOne(och => och.Clinic)
                .WithMany(c => c.OpenCloseHours)
                .HasForeignKey(och => och.ClinicId)
                .IsRequired()
                .HasConstraintName("FK_OpenCloseHours_Clinic");
        }
    }
}
