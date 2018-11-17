﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SistemaTurnos.Database.ClinicModel;
using SistemaTurnos.Database.HairdressingModel;
using SistemaTurnos.Database.Model;
using SistemaTurnos.Database.ModelData;
using SistemaTurnos.Database.TypeConfigurations;
using SistemaTurnos.Database.TypeConfigurations.Clinic;
using SistemaTurnos.Database.TypeConfigurations.Hairdressing;
using Microsoft.EntityFrameworkCore.InMemory.Infrastructure.Internal;

namespace SistemaTurnos.Database
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
    {
        private const string databaseServer = "localhost";
        private const string databaseName = "sistematurnos";
        private const string databaseUser = "root";
        private const string databasePass = "fernando";
        //private const string databasePass = "1682951";
        //private const string databasePass = "tito1234H6*";

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
            : base(options.FindExtension<InMemoryOptionsExtension>() == null ?
                  new DbContextOptionsBuilder<ApplicationDbContext>().UseLazyLoadingProxies().UseMySql(GetConnectionString()).Options : options)
        {
        }

        public ApplicationDbContext() : base(new DbContextOptionsBuilder<ApplicationDbContext>().UseLazyLoadingProxies().UseMySql(GetConnectionString()).Options)
        {
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseLazyLoadingProxies().UseMySql(GetConnectionString
        private static string GetConnectionString() => $"Server={databaseServer};database={databaseName};uid={databaseUser};pwd={databasePass};pooling=true;";

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Application
            modelBuilder.ApplyConfiguration(new ApplicationUserTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ApplicationRoleTypeConfiguration());

            // Clinic
            modelBuilder.ApplyConfiguration(new Clinic_AppointmentTypeConfiguration());
            modelBuilder.ApplyConfiguration(new Clinic_ClientTypeConfiguration());
            modelBuilder.ApplyConfiguration(new Clinic_DoctorTypeConfiguration());
            modelBuilder.ApplyConfiguration(new Clinic_EmployeeTypeConfiguration());
            modelBuilder.ApplyConfiguration(new Clinic_MedicalInsuranceTypeConfiguration());
            modelBuilder.ApplyConfiguration(new Clinic_MedicalPlanTypeConfiguration());
            modelBuilder.ApplyConfiguration(new Clinic_PatientTypeConfiguration());
            modelBuilder.ApplyConfiguration(new Clinic_RatingTypeConfiguration());
            modelBuilder.ApplyConfiguration(new Clinic_SpecialtyTypeConfiguration());
            modelBuilder.ApplyConfiguration(new Clinic_SubspecialtyTypeConfiguration());
            modelBuilder.ApplyConfiguration(new Clinic_WorkingHoursTypeConfiguration());
            modelBuilder.ApplyConfiguration(new Clinic_OpenCloseHoursTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ClinicTypeConfiguration());
            modelBuilder.ApplyConfiguration(new Clinic_ClientFavoriteTypeConfiguration());

            // Hairdressing
            modelBuilder.ApplyConfiguration(new Hairdressing_AppointmentTypeConfiguration());
            modelBuilder.ApplyConfiguration(new Hairdressing_ProfessionalTypeConfiguration());
            modelBuilder.ApplyConfiguration(new Hairdressing_EmployeeTypeConfiguration());
            modelBuilder.ApplyConfiguration(new Hairdressing_PatientTypeConfiguration());
            modelBuilder.ApplyConfiguration(new Hairdressing_RatingTypeConfiguration());
            modelBuilder.ApplyConfiguration(new Hairdressing_SpecialtyTypeConfiguration());
            modelBuilder.ApplyConfiguration(new Hairdressing_SubspecialtyTypeConfiguration());
            modelBuilder.ApplyConfiguration(new Hairdressing_WorkingHoursTypeConfiguration());
            modelBuilder.ApplyConfiguration(new Hairdressing_OpenCloseHoursTypeConfiguration());
            modelBuilder.ApplyConfiguration(new HairdressingTypeConfiguration());
            modelBuilder.ApplyConfiguration(new Hairdressing_ClientFavoriteTypeConfiguration());
        }
        
        // Clinic Model
        public DbSet<Clinic_Specialty> Clinic_Specialties { get; set; }

        public DbSet<Clinic_Patient> Clinic_Patients { get; set; }

        public DbSet<Clinic_Doctor> Clinic_Doctors { get; set; }

        public DbSet<Clinic_WorkingHours> Clinic_WorkingHours { get; set; }

        public DbSet<Clinic_Appointment> Clinic_Appointments { get; set; }

        public DbSet<Clinic_Subspecialty> Clinic_Subspecialties { get; set; }

        public DbSet<Clinic_Rating> Clinic_Ratings { get; set; }

        public DbSet<Clinic> Clinics { get; set; }

        public DbSet<Clinic_Employee> Clinic_Employees { get; set; }

        public DbSet<SystemClient> Clients { get; set; }

        public DbSet<Clinic_MedicalInsurance> Clinic_MedicalInsurances { get; set; }

        public DbSet<Clinic_MedicalPlan> Clinic_MedicalPlans { get; set; }

        public DbSet<Clinic_OpenCloseHours> Clinic_OpenCloseHours { get; set; }

        public DbSet<Clinic_ClientFavorite> Clinic_ClientFavorites { get; set; }

        // Model Data
        public DbSet<SpecialtyData> Specialties { get; set; }

        public DbSet<SubspecialtyData> Subspecialties { get; set; }

        public DbSet<MedicalInsuranceData> MedicalInsurances { get; set; }

        public DbSet<MedicalPlanData> MedicalPlans { get; set; }

        public DbSet<City> Cities { get; set; }

        // Hairdressing Model
        public DbSet<Hairdressing_Specialty> Hairdressing_Specialties { get; set; }

        public DbSet<Hairdressing_Patient> Hairdressing_Patients { get; set; }

        public DbSet<Hairdressing_Professional> Hairdressing_Professionals { get; set; }

        public DbSet<Hairdressing_WorkingHours> Hairdressing_WorkingHours { get; set; }

        public DbSet<Hairdressing_Appointment> Hairdressing_Appointments { get; set; }

        public DbSet<Hairdressing_Subspecialty> Hairdressing_Subspecialties { get; set; }

        public DbSet<Hairdressing_Rating> Hairdressing_Ratings { get; set; }

        public DbSet<Hairdressing> Hairdressings { get; set; }

        public DbSet<Hairdressing_Employee> Hairdressing_Employees { get; set; }

        public DbSet<Hairdressing_OpenCloseHours> Hairdressing_OpenCloseHours { get; set; }

        public DbSet<Hairdressing_ClientFavorite> Hairdressing_ClientFavorites { get; set; }
    }
}