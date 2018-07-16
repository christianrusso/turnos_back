using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaTurnos.WebApplication.Database.ClinicModel;

namespace SistemaTurnos.WebApplication.Database.TypeConfigurations.Clinic
{
    public class Clinic_ClientTypeConfiguration : IEntityTypeConfiguration<Clinic_Client>
    {
        public void Configure(EntityTypeBuilder<Clinic_Client> builder)
        {
            // User
            builder
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .HasConstraintName("FK_Client_User");
        }
    }
}
