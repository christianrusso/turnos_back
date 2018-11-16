using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ClinicEntity = SistemaTurnos.Database.ClinicModel.Clinic;

namespace SistemaTurnos.Database.TypeConfigurations.Clinic
{
    public class ClinicTypeConfiguration : IEntityTypeConfiguration<ClinicEntity>
    {
        public void Configure(EntityTypeBuilder<ClinicEntity> builder)
        {
            // Ciudad
            builder
                .HasOne(c => c.City)
                .WithMany()
                .HasForeignKey(c => c.CityId)
                .HasConstraintName("FK_Clinic_City");

            // User
            builder
                .HasOne(c => c.User)
                .WithOne()
                .HasForeignKey<ClinicEntity>(c => c.UserId)
                .HasConstraintName("FK_Clinic_User");

        }
    }
}
