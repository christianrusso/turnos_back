using System.ComponentModel.DataAnnotations;

namespace SistemaTurnos.WebApplication.WebApi.Dto.Specialty
{
    public class AddSpecialtyDto : BaseDto
    {
        [Required]
        [StringLength(50, MinimumLength = 4)]
        public string Description { get; set; }
    }
}
