using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaTurnos.Database.ClinicModel;

namespace SistemaTurnos.Database.TypeConfigurations.Clinic
{
    public class Clinic_MedicalPlanTypeConfiguration : IEntityTypeConfiguration<Clinic_MedicalPlan>
    {
        public void Configure(EntityTypeBuilder<Clinic_MedicalPlan> builder)
        {
            // Medical Plan Data
            builder
                .HasOne(mp => mp.Data)
                .WithMany()
                .HasForeignKey(mp => mp.DataId)
                .HasConstraintName("FK_MedicalPlan_Data");

            // User
            builder
                .HasOne(mp => mp.User)
                .WithMany()
                .HasForeignKey(mi => mi.UserId)
                .HasConstraintName("FK_MedicalPlan_User");
        }
    }
}
