using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ClinicEntity = SistemaTurnos.WebApplication.Database.ClinicModel.Clinic;

namespace SistemaTurnos.WebApplication.Database.TypeConfigurations.Clinic
{
    public class ClinicTypeConfiguration : IEntityTypeConfiguration<ClinicEntity>
    {
        public void Configure(EntityTypeBuilder<ClinicEntity> builder)
        {
            // User
            builder
                .HasOne(c => c.User)
                .WithOne()
                .HasForeignKey<ClinicEntity>(c => c.UserId)
                .HasConstraintName("FK_Clinic_User");
        }
    }
}
