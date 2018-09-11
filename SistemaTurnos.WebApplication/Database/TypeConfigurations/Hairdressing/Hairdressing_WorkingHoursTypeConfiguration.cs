using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaTurnos.WebApplication.Database.HairdressingModel;

namespace SistemaTurnos.WebApplication.Database.TypeConfigurations.Hairdressing
{
    public class Hairdressing_WorkingHoursTypeConfiguration : IEntityTypeConfiguration<Hairdressing_WorkingHours>
    {
        public void Configure(EntityTypeBuilder<Hairdressing_WorkingHours> builder)
        {
            // professional
            builder
                .HasOne(wh => wh.Professional)
                .WithMany(d => d.WorkingHours)
                .HasForeignKey(wh => wh.ProfessionalId)
                .IsRequired()
                .HasConstraintName("FK_Hairdressing_WorkingHours_Doctor");
        }
    }
}
