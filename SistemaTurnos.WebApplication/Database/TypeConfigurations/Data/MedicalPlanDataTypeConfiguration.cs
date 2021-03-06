﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaTurnos.WebApplication.Database.ModelData;

namespace SistemaTurnos.WebApplication.Database.TypeConfigurations.Data
{
    public class MedicalPlanDataTypeConfiguration : IEntityTypeConfiguration<MedicalPlanData>
    {
        public void Configure(EntityTypeBuilder<MedicalPlanData> builder)
        {
            builder
                .HasOne(mpd => mpd.MedicalInsuranceData)
                .WithMany(mid => mid.MedicalPlans)
                .HasForeignKey(mi => mi.MedicalInsuranceDataId)
                .IsRequired()
                .HasConstraintName("FK_MedicalPlanData_MedicalInsuranceData");
        }
    }
}
