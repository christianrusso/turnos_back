using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaTurnos.Database.HairdressingModel;

namespace SistemaTurnos.Database.TypeConfigurations.Hairdressing
{
    public class Hairdressing_EmployeeTypeConfiguration : IEntityTypeConfiguration<Hairdressing_Employee>
    {
        public void Configure(EntityTypeBuilder<Hairdressing_Employee> builder)
        {
            // User
            builder
                .HasOne(e => e.User)
                .WithOne()
                .HasForeignKey<Hairdressing_Employee>(e => e.UserId)
                .HasConstraintName("FK_HairdressingEmployee_User");

            // Owner user
            builder
                .HasOne(e => e.OwnerUser)
                .WithMany()
                .HasForeignKey(e => e.OwnerUserId)
                .HasConstraintName("FK_HairdressingEmployee_OwnerUser");
        }
    }
}
