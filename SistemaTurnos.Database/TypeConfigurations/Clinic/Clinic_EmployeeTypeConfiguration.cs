using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaTurnos.Database.ClinicModel;

namespace SistemaTurnos.Database.TypeConfigurations.Clinic
{
    public class Clinic_EmployeeTypeConfiguration : IEntityTypeConfiguration<Clinic_Employee>
    {
        public void Configure(EntityTypeBuilder<Clinic_Employee> builder)
        {
            // User
            builder
                .HasOne(e => e.User)
                .WithOne()
                .HasForeignKey<Clinic_Employee>(e => e.UserId)
                .HasConstraintName("FK_Employee_User");

            // Owner user
            builder
                .HasOne(e => e.OwnerUser)
                .WithMany()
                .HasForeignKey(e => e.OwnerUserId)
                .HasConstraintName("FK_Employee_OwnerUser");
        }
    }
}
