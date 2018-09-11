using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaTurnos.WebApplication.Database.HairdressingModel;

namespace SistemaTurnos.WebApplication.Database.TypeConfigurations.Hairdressing
{
    public class Hairdressing_SpecialtyTypeConfiguration : IEntityTypeConfiguration<Hairdressing_Specialty>
    {
        public void Configure(EntityTypeBuilder<Hairdressing_Specialty> builder)
        {
            // Specialty Data
            builder
                .HasOne(s => s.Data)
                .WithMany()
                .HasForeignKey(s => s.DataId)
                .HasConstraintName("FK_Hairdressing_Specialty_Data");

            // User
            builder
                .HasOne(s => s.User)
                .WithMany()
                .HasForeignKey(s => s.UserId)
                .HasConstraintName("FK_Hairdressing_Specialty_User");
        }
    }
}
