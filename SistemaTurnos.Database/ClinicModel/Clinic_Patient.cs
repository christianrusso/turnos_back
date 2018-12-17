using SistemaTurnos.Database.Model;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaTurnos.Database.ClinicModel
{
    public class Clinic_Patient
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int MedicalPlanId { get; set; }

        public virtual Clinic_MedicalPlan MedicalPlan { get; set; }

        public virtual List<Clinic_Appointment> Appointments { get; set; }

        [Required]
        public int UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        [Required]
        public int ClientId { get; set; }

        public virtual SystemClient Client { get; set; }

        public string FullName => Client.FullName;
    }
}
