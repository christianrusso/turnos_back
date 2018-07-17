using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SistemaTurnos.WebApplication.Database.Model;

namespace SistemaTurnos.WebApplication.Database.ClinicModel
{
    public class Clinic_Client
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        public virtual List<Clinic_Patient> Patients { get; set; }
    }
}
