using System.ComponentModel.DataAnnotations;

namespace SistemaTurnos.WebApplication.WebApi.Dto.Specialty
{
    public class RemoveSpecialtyDto : BaseDto
    {
        [Required]
        public int Id { get; set; }
    }
}
