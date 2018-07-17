using SistemaTurnos.WebApplication.Database.Model;
using SistemaTurnos.WebApplication.Database.ModelData;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaTurnos.WebApplication.Database.ClinicModel
{
    public class Clinic_MedicalInsurance
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int DataId { get; set; }

        public virtual MedicalInsuranceData Data { get; set; }

        public virtual List<Clinic_MedicalPlan> MedicalPlans { get; set; }

        [Required]
        public int UserId { get; set; }

        public virtual ApplicationUser User { get; set; }
    }
}
