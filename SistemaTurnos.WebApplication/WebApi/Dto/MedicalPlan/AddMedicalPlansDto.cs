using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SistemaTurnos.WebApplication.WebApi.Dto.MedicalPlan
{
    public class AddMedicalPlansDto : BaseDto
    {
        [Required]
        public int MedicalInsuranceId { get; set; }

        [Required]
        public List<MedicalPlanDto> MedicalPlans { get; set; }
    }
}
