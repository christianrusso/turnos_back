using System.ComponentModel.DataAnnotations;

namespace SistemaTurnos.WebApplication.WebApi.Dto.MedicalPlan
{
    public class AddMedicalPlanDto : BaseDto
    {
        [Required]
        public string Description { get; set; }

        [Required]
        public int MedicalInsuranceId { get; set; }
    }
}
