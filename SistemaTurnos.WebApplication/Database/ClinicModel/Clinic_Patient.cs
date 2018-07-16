using SistemaTurnos.WebApplication.Database.Model;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaTurnos.WebApplication.Database.ClinicModel
{
    public class Clinic_Patient
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 4)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 4)]
        public string LastName { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 4)]
        public string Address { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 4)]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 4)]
        public string Dni { get; set; }

        public int MedicalPlanId { get; set; }

        public Clinic_MedicalPlan MedicalPlan { get; set; }

        public List<Clinic_Appointment> Appointments { get; set; }

        [Required]
        public int UserId { get; set; }

        public ApplicationUser User { get; set; }

        [Required]
        public int ClientId { get; set; }

        public Clinic_Client Client { get; set; }

        public string FullName => $"{LastName} {FirstName}";
    }
}
