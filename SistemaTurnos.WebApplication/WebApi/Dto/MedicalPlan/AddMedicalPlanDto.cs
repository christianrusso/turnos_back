using System.ComponentModel.DataAnnotations;

namespace SistemaTurnos.WebApplication.WebApi.Dto.MedicalPlan
{
    public class AddMedicalPlanDto : BaseDto
    {
        [Required]
        [MaxLength(25)]
        public string Description { get; set; }

        [Required]
        public int MedicalInsuranceId { get; set; }
    }
}
