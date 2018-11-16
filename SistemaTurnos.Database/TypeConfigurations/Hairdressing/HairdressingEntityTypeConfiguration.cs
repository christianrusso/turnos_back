using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HairdressingEntity = SistemaTurnos.Database.HairdressingModel.Hairdressing;

namespace SistemaTurnos.Database.TypeConfigurations.Hairdressing
{
    public class HairdressingTypeConfiguration : IEntityTypeConfiguration<HairdressingEntity>
    {
        public void Configure(EntityTypeBuilder<HairdressingEntity> builder)
        {
            // Ciudad
            builder
                .HasOne(c => c.City)
                .WithMany()
                .HasForeignKey(c => c.CityId)
                .HasConstraintName("FK_Hairdressing_City");

            // User
            builder
                .HasOne(c => c.User)
                .WithOne()
                .HasForeignKey<HairdressingEntity>(c => c.UserId)
                .HasConstraintName("FK_Hairdressing_User");

        }
    }
}
