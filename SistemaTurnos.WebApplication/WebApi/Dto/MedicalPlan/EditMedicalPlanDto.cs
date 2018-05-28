using System.ComponentModel.DataAnnotations;

namespace SistemaTurnos.WebApplication.WebApi.Dto.MedicalPlan
{
    public class EditMedicalPlanDto : BaseDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(25)]
        public string Description { get; set; }
    }
}
