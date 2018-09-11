﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaTurnos.WebApplication.Database.HairdressingModel;

namespace SistemaTurnos.WebApplication.Database.TypeConfigurations.Hairdressing
{
    public class Hairdressing_PatientTypeConfiguration : IEntityTypeConfiguration<Hairdressing_Patient>
    {
        public void Configure(EntityTypeBuilder<Hairdressing_Patient> builder)
        {
            // User
            builder
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .HasConstraintName("FK_Hairdressing_Patient_User");

            // Client
            builder
                .HasOne(p => p.Client)
                .WithMany(c => c.Patients)
                .HasForeignKey(p => p.ClientId)
                .HasConstraintName("FK_Hairdressing_Patient_Client");
        }
    }
}
