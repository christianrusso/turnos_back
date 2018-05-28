using System.ComponentModel.DataAnnotations;

namespace SistemaTurnos.WebApplication.WebApi.Dto.MedicalPlan
{
    public class MedicalPlanDto : BaseDto
    {
        [Required]
        [MaxLength(25)]
        public string Description { get; set; }
    }
}