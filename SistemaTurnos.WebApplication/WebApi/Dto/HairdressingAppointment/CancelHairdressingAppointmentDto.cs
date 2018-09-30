using System.ComponentModel.DataAnnotations;

namespace SistemaTurnos.WebApplication.WebApi.Dto.HairdressingAppointment
{
    public class CancelHairdressingAppointmentDto : BaseDto
    {
        [Required]
        public int  Id { get; set; }

        [Required]
        public string Comment { get; set; }
    }
}
