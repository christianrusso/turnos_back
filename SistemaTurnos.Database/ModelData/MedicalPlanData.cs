using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaTurnos.Database.ModelData
{
    public class MedicalPlanData
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Description { get; set; }

        [Required]
        public int MedicalInsuranceDataId { get; set; }

        public virtual MedicalInsuranceData MedicalInsuranceData { get; set; }
    }
}
