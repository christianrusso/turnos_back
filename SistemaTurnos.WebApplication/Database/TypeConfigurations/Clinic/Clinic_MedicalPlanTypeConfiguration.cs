using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaTurnos.WebApplication.Database.ClinicModel;

namespace SistemaTurnos.WebApplication.Database.TypeConfigurations.Clinic
{
    public class Clinic_MedicalPlanTypeConfiguration : IEntityTypeConfiguration<Clinic_MedicalPlan>
    {
        public void Configure(EntityTypeBuilder<Clinic_MedicalPlan> builder)
        {
            // Medical Insurance
            builder
                .HasOne(mp => mp.MedicalInsurance)
                .WithMany(mi => mi.MedicalPlans)
                .HasForeignKey(mp => mp.MedicalInsuranceId)
                .HasConstraintName("FK_MedicalPlan_MedicalInsurance");
        }
    }
}
