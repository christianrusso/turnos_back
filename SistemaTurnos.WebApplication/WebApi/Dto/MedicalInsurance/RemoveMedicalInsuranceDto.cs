using System.ComponentModel.DataAnnotations;

namespace SistemaTurnos.WebApplication.WebApi.Dto.MedicalInsurance
{
    public class RemoveMedicalInsuranceDto : BaseDto
    {
        [Required]
        public int Id { get; set; }
    }
}
