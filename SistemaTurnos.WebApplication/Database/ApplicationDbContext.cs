using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SistemaTurnos.WebApplication.Database.ClinicModel;
using SistemaTurnos.WebApplication.Database.Model;
using SistemaTurnos.WebApplication.Database.ModelData;
using SistemaTurnos.WebApplication.Database.TypeConfigurations;
using SistemaTurnos.WebApplication.Database.TypeConfigurations.Clinic;

namespace SistemaTurnos.WebApplication.Database
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
    {
        private const string databaseServer = "localhost";
        private const string databaseName = "sistematurnos";
        private const string databaseUser = "root";
        private const string databasePass = "1682951";

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseLazyLoadingProxies().UseMySql(GetConnectionString());

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

        public DbSet<Clinic_Client> Clinic_Clients { get; set; }

        public DbSet<Clinic_MedicalInsurance> Clinic_MedicalInsurances { get; set; }

        public DbSet<Clinic_MedicalPlan> Clinic_MedicalPlans { get; set; }

        public DbSet<Clinic_OpenCloseHours> Clinic_OpenCloseHours { get; set; }

        // Model Data
        public DbSet<SpecialtyData> Specialties { get; set; }

        public DbSet<SubspecialtyData> Subspecialties { get; set; }

        public DbSet<MedicalInsuranceData> MedicalInsurances { get; set; }

        public DbSet<MedicalPlanData> MedicalPlans { get; set; }

        public DbSet<City> Cities { get; set; }
    }
}
