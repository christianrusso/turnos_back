using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaTurnos.Database.HairdressingModel;

namespace SistemaTurnos.Database.TypeConfigurations.Hairdressing
{
    public class Hairdressing_ProfessionalTypeConfiguration : IEntityTypeConfiguration<Hairdressing_Professional>
    {
        public void Configure(EntityTypeBuilder<Hairdressing_Professional> builder)
        {
            // User
            builder
                .HasOne(d => d.User)
                .WithMany()
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Hairdressing_Professional_User");
        }
    }
}
