﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaTurnos.Database.ClinicModel;

namespace SistemaTurnos.Database.TypeConfigurations.Clinic
{
    public class Clinic_ClientTypeConfiguration : IEntityTypeConfiguration<SystemClient>
    {
        public void Configure(EntityTypeBuilder<SystemClient> builder)
        {
            // User
            builder
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .HasConstraintName("FK_Client_User");
        }
    }
}