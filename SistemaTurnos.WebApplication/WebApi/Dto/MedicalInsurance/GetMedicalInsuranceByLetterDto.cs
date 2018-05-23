using System.ComponentModel.DataAnnotations;

namespace SistemaTurnos.WebApplication.WebApi.Dto.MedicalInsurance
{
    public class GetMedicalInsuranceByLetterDto : BaseDto
    {
        [Required]
        public char Letter { get; set; }
    }
}
