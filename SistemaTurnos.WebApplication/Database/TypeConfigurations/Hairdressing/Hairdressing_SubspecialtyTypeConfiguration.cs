using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaTurnos.WebApplication.Database.HairdressingModel;

namespace SistemaTurnos.WebApplication.Database.TypeConfigurations.Hairdressing
{
    public class Hairdressing_SubspecialtyTypeConfiguration : IEntityTypeConfiguration<Hairdressing_Subspecialty>
    {
        public void Configure(EntityTypeBuilder<Hairdressing_Subspecialty> builder)
        {
            // Subspecialty Data
            builder
                .HasOne(ssp => ssp.Data)
                .WithMany()
                .HasForeignKey(ssp => ssp.DataId)
                .HasConstraintName("FK_Hairdressing_SubSpecialty_Data");

            // Specialty
            builder
                .HasOne(ssp => ssp.Specialty)
                .WithMany(s => s.Subspecialties)
                .HasForeignKey(ssp => ssp.SpecialtyId)
                .IsRequired()
                .HasConstraintName("FK_Hairdressing_SubSpecialty_Specialty");

            // User
            builder
                .HasOne(ssp => ssp.User)
                .WithMany()
                .HasForeignKey(ssp => ssp.UserId)
                .HasConstraintName("FK_Hairdressing_Subspecialty_User");
        }
    }
}
