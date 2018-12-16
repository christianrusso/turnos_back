using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaTurnos.Database.HairdressingModel;

namespace SistemaTurnos.Database.TypeConfigurations.Clinic
{
    public class Hairdressing_ClientFavoriteTypeConfiguration : IEntityTypeConfiguration<Hairdressing_ClientFavorite>
    {
        public void Configure(EntityTypeBuilder<Hairdressing_ClientFavorite> builder)
        {
            builder.HasKey(cfc => new { cfc.ClientId, cfc.HairdressingId });

            builder
                .HasOne(cfc => cfc.Client)
                .WithMany(c => c.FavoriteHairdressing)
                .HasForeignKey(cfc => cfc.ClientId);

            builder
                .HasOne(cfc => cfc.Hairdressing)
                .WithMany()
                .HasForeignKey(cfc => cfc.HairdressingId);
        }
    }
}
