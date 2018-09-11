using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaTurnos.WebApplication.Database.HairdressingModel;

namespace SistemaTurnos.WebApplication.Database.TypeConfigurations.Hairdressing
{
    public class Hairdressing_ProfessionalTypeConfiguration : IEntityTypeConfiguration<Hairdressing_Professional>
    {
        public void Configure(EntityTypeBuilder<Hairdressing_Professional> builder)
        {
            // Specialty
            builder
                .HasOne(d => d.Specialty)
                .WithMany(s => s.Professionals)
                .HasForeignKey(d => d.SpecialtyId)
                .IsRequired()
                .HasConstraintName("FK_Hairdressing_Professional_Specialty");

            // Subspecialty
            builder
                .HasOne(d => d.Subspecialty)
                .WithMany()
                .HasForeignKey(d => d.SubspecialtyId)
                .HasConstraintName("FK_Hairdressing_Professional_Subspecialty");

            // User
            builder
                .HasOne(d => d.User)
                .WithMany()
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Hairdressing_Professional_User");
        }
    }
}
