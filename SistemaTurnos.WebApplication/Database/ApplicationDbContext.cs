using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SistemaTurnos.WebApplication.Database.Model;

namespace SistemaTurnos.WebApplication.Database
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
    {
        private const string databaseServer = "localhost";
        private const string databaseName = "sistematurnos";
        private const string databaseUser = "root";
        private const string databasePass = "1682951";


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseMySql(GetConnectionString());

        private static string GetConnectionString() => $"Server={databaseServer};database={databaseName};uid={databaseUser};pwd={databasePass};pooling=true;";

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.ToTable(name: "AspNetUser");
            });

            modelBuilder.Entity<ApplicationRole>(entity =>
            {
                entity.ToTable(name: "AspNetRoles");
            });

            modelBuilder.Entity<Doctor>()
                .HasOne(d => d.Specialty)
                .WithMany(s => s.Doctors)
                .HasForeignKey(d => d.SpecialtyId)
                .IsRequired()
                .HasConstraintName("FK_Doctor_Specialty");

            modelBuilder.Entity<Doctor>()
                .HasOne(d => d.Subspecialty)
                .WithMany()
                .HasForeignKey(d => d.SubspecialtyId)
                .HasConstraintName("FK_Doctor_Subspecialty");

            modelBuilder.Entity<WorkingHours>()
                .HasOne(wh => wh.Doctor)
                .WithMany(d => d.WorkingHours)
                .HasForeignKey(wh => wh.DoctorId)
                .IsRequired()
                .HasConstraintName("FK_WorkingHours_Doctor");

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Doctor)
                .WithMany(d => d.Appointments)
                .HasForeignKey(a => a.DoctorId)
                .IsRequired()
                .HasConstraintName("FK_Appointment_Doctor");

            modelBuilder.Entity<Subspecialty>()
                .HasOne(ssp => ssp.Specialty)
                .WithMany(s => s.Subspecialties)
                .HasForeignKey(ssp => ssp.SpecialtyId)
                .IsRequired()
                .HasConstraintName("FK_SubSpecialty_Specialty");


            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Rating)
                .WithOne(r => r.Appointment)
                .HasForeignKey<Rating>(r => r.AppointmentId)
                .HasConstraintName("FK_Rating_Appointment");

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Patient)
                .WithMany(p => p.Appointments)
                .HasForeignKey(a => a.PatientId)
                .HasConstraintName("FK_Appointment_Patient");

            modelBuilder.Entity<MedicalPlan>()
                .HasOne(mp => mp.MedicalInsurance)
                .WithMany(mi => mi.MedicalPlans)
                .HasForeignKey(mp => mp.MedicalInsuranceId)
                .HasConstraintName("FK_MedicalPlan_MedicalInsurance");

            modelBuilder.Entity<Patient>()
                .HasOne(p => p.MedicalPlan)
                .WithMany(mp => mp.Patients)
                .HasForeignKey(p => p.MedicalPlanId)
                .HasConstraintName("FK_Patient_MedicalPlan");

            // Configuracion de usuarios
            modelBuilder.Entity<Clinic>()
                .HasOne(c => c.User)
                .WithOne()
                .HasForeignKey<Clinic>(c => c.UserId)
                .HasConstraintName("FK_Clinic_User");

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.User)
                .WithOne()
                .HasForeignKey<Employee>(e => e.UserId)
                .HasConstraintName("FK_Employee_User");

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.OwnerUser)
                .WithMany()
                .HasForeignKey(e => e.OwnerUserId)
                .HasConstraintName("FK_Employee_OwnerUser");

            modelBuilder.Entity<Client>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .HasConstraintName("FK_Client_User");

            modelBuilder.Entity<Doctor>()
                .HasOne(d => d.User)
                .WithMany()
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Doctor_User");

            modelBuilder.Entity<Patient>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .HasConstraintName("FK_Patient_User");

            modelBuilder.Entity<Rating>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .HasConstraintName("FK_Rating_User");

            modelBuilder.Entity<Specialty>()
                .HasOne(s => s.User)
                .WithMany()
                .HasForeignKey(s => s.UserId)
                .HasConstraintName("FK_Specialty_User");

            modelBuilder.Entity<MedicalInsurance>()
                .HasOne(s => s.User)
                .WithMany()
                .HasForeignKey(s => s.UserId)
                .HasConstraintName("FK_MedicalInsurance_User");

            modelBuilder.Entity<Subspecialty>()
                .HasOne(ss => ss.User)
                .WithMany()
                .HasForeignKey(ss => ss.UserId)
                .HasConstraintName("FK_Subspecialty_User");

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .HasConstraintName("FK_Appointment_User");

            modelBuilder.Entity<Patient>()
                .HasOne(p => p.Client)
                .WithMany(c => c.Patients)
                .HasForeignKey(p => p.ClientId)
                .HasConstraintName("FK_Patient_Client");
        }

        public DbSet<Specialty> Specialties { get; set; }

        public DbSet<Patient> Patients { get; set; }

        public DbSet<Doctor> Doctors { get; set; }

        public DbSet<WorkingHours> WorkingHours { get; set; }

        public DbSet<Appointment> Appointments { get; set; }

        public DbSet<Subspecialty> Subspecialties { get; set; }

        public DbSet<Rating> Ratings { get; set; }

        public DbSet<Clinic> Clinics { get; set; }

        public DbSet<Employee> Employees { get; set; }

        public DbSet<Client> Clients { get; set; }

        public DbSet<MedicalInsurance> MedicalInsurances { get; set; }

        public DbSet<MedicalPlan> MedicalPlans { get; set; }
    }
}
