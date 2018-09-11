using System.ComponentModel.DataAnnotations;

namespace SistemaTurnos.WebApplication.WebApi.Dto.HairdressingProfessional
{
    public class EnableDisableHairdressingProfessionalDto : BaseDto
    {
        [Required]
        public int Id { get; set; }
    }
}
