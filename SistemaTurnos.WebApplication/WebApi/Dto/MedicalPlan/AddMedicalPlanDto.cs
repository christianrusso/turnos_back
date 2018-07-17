using System.ComponentModel.DataAnnotations;

namespace SistemaTurnos.WebApplication.WebApi.Dto.MedicalPlan
{
    public class AddMedicalPlanDto : BaseDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public int MedicalInsuranceId { get; set; }
    }
}
