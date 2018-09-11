using System.ComponentModel.DataAnnotations;

namespace SistemaTurnos.WebApplication.WebApi.Dto.HairdressingProfessional
{
    public class RemoveHairdressingProfessionalDto : BaseDto
    {
        [Required]
        public int Id { get; set; }
    }
}
