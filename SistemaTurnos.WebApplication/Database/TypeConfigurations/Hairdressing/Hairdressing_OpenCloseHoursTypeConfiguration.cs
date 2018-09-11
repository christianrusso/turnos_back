using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaTurnos.WebApplication.Database.HairdressingModel;

namespace SistemaTurnos.WebApplication.Database.TypeConfigurations.Hairdressing
{
    public class Hairdressing_OpenCloseHoursTypeConfiguration : IEntityTypeConfiguration<Hairdressing_OpenCloseHours>
    {
        public void Configure(EntityTypeBuilder<Hairdressing_OpenCloseHours> builder)
        {
            // Hairdressing
            builder
                .HasOne(och => och.Hairdressing)
                .WithMany(c => c.OpenCloseHours)
                .HasForeignKey(och => och.HairdressingId)
                .IsRequired()
                .HasConstraintName("FK_HairdressingIdOpenCloseHours_Clinic");
        }
    }
}
