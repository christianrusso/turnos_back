using System.ComponentModel.DataAnnotations;

namespace SistemaTurnos.WebApplication.WebApi.Dto.MedicalPlan
{
    public class MedicalPlanDto : BaseDto
    {
        [Required]
        public string Description { get; set; }
    }
}