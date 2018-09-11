using System.ComponentModel.DataAnnotations;

namespace SistemaTurnos.WebApplication.WebApi.Dto.HairdressingPatient
{
    public class RemoveHairdressingPatientDto : BaseDto
    {
        [Required]
        public int Id { get; set; }
    }
}
