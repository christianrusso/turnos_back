using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaTurnos.Database.ModelData;

namespace SistemaTurnos.Database.TypeConfigurations.Data
{
    public class ImageTypeConfiguration : IEntityTypeConfiguration<Image>
    {
        public void Configure(EntityTypeBuilder<Image> builder)
        {
            // User
            builder
                .HasOne(i => i.User)
                .WithMany()
                .HasForeignKey(i => i.UserId)
                .HasConstraintName("FK_Image_User");
        }
    }
}
