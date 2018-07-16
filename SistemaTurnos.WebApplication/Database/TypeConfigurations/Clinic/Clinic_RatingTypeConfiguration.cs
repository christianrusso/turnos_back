using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaTurnos.WebApplication.Database.ClinicModel;

namespace SistemaTurnos.WebApplication.Database.TypeConfigurations.Clinic
{
    public class Clinic_RatingTypeConfiguration : IEntityTypeConfiguration<Clinic_Rating>
    {
        public void Configure(EntityTypeBuilder<Clinic_Rating> builder)
        {
            // User
            builder
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .HasConstraintName("FK_Rating_User");
        }
    }
}
