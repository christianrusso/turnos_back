using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaTurnos.Database.ClinicModel;

namespace SistemaTurnos.Database.TypeConfigurations.Clinic
{
    public class Clinic_ClientFavoriteTypeConfiguration : IEntityTypeConfiguration<Clinic_ClientFavorite>
    {
        public void Configure(EntityTypeBuilder<Clinic_ClientFavorite> builder)
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
