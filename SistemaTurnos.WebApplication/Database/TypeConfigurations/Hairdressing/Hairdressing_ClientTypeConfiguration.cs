using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaTurnos.WebApplication.Database.HairdressingModel;

namespace SistemaTurnos.WebApplication.Database.TypeConfigurations.Hairdressing
{
    public class Hairdressing_ClientTypeConfiguration : IEntityTypeConfiguration<Hairdressing_Client>
    {
        public void Configure(EntityTypeBuilder<Hairdressing_Client> builder)
        {
            // User
            builder
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .HasConstraintName("FK_HairdressingClient_User");
        }
    }
}
