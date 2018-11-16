using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaTurnos.Database.HairdressingModel;

namespace SistemaTurnos.Database.TypeConfigurations.Hairdressing
{
    public class Hairdressing_RatingTypeConfiguration : IEntityTypeConfiguration<Hairdressing_Rating>
    {
        public void Configure(EntityTypeBuilder<Hairdressing_Rating> builder)
        {
            // User
            builder
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .HasConstraintName("FK_Hairdressing_Rating_User");
        }
    }
}
