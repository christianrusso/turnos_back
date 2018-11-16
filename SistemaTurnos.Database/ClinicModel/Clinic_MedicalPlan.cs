using SistemaTurnos.Database.Model;
using SistemaTurnos.Database.ModelData;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaTurnos.Database.ClinicModel
{
    public class Clinic_MedicalPlan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int DataId { get; set; }

        public virtual MedicalPlanData Data { get; set; }

        [Required]
        public int MedicalInsuranceId { get; set; }

        [Required]
        public virtual Clinic_MedicalInsurance MedicalInsurance { get; set; }

        public virtual List<Clinic_Patient> Patients { get; set; }

        [Required]
        public int UserId { get; set; }

        public virtual ApplicationUser User { get; set; }
    }
}
