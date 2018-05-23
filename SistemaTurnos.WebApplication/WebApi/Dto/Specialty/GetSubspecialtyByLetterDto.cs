using System.ComponentModel.DataAnnotations;

namespace SistemaTurnos.WebApplication.WebApi.Dto.Specialty
{
    public class GetSubspecialtyByLetterDto : BaseDto
    {
        [Required]
        public char Letter { get; set; }
    }
}
