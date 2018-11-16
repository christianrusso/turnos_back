using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaTurnos.Database.ModelData;

namespace SistemaTurnos.Database.TypeConfigurations.Data
{
    public class SubspecialtyDataTypeConfiguration : IEntityTypeConfiguration<SubspecialtyData>
    {
        public void Configure(EntityTypeBuilder<SubspecialtyData> builder)
        {
            // Specialty
            builder
                .HasOne(ssp => ssp.SpecialtyData)
                .WithMany(s => s.Subspecialties)
                .HasForeignKey(ssp => ssp.SpecialtyDataId)
                .IsRequired()
                .HasConstraintName("FK_SubSpecialtyData_SpecialtyData");
        }
    }
}
