using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaTurnos.WebApplication.Database.ClinicModel;

namespace SistemaTurnos.WebApplication.Database.TypeConfigurations.Clinic
{
    public class Clinic_ClientFavoriteClinicsTypeConfiguration : IEntityTypeConfiguration<Clinic_ClientFavoriteClinics>
    {
        public void Configure(EntityTypeBuilder<Clinic_ClientFavoriteClinics> builder)
        {
            builder.HasKey(cfc => new { cfc.ClientId, cfc.ClinicId });

            builder
                .HasOne(cfc => cfc.Client)
                .WithMany(c => c.FavoriteClinics)
                .HasForeignKey(cfc => cfc.ClientId);

            builder
                .HasOne(cfc => cfc.Clinic)
                .WithMany()
                .HasForeignKey(cfc => cfc.ClinicId);
        }
    }
}
