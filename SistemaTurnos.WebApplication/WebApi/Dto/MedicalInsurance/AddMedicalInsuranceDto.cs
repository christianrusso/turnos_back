using System.ComponentModel.DataAnnotations;

namespace SistemaTurnos.WebApplication.WebApi.Dto.MedicalInsurance
{
    public class AddMedicalInsuranceDto : BaseDto
    {
        [Required]
        [StringLength(50, MinimumLength = 4)]
        public string Description { get; set; }
    }
}
