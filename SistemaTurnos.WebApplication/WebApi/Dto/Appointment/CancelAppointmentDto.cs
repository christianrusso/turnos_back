using System.ComponentModel.DataAnnotations;

namespace SistemaTurnos.WebApplication.WebApi.Dto.Appointment
{
    public class CancelAppointmentDto : BaseDto
    {
        [Required]
        public int  Id { get; set; }
    }
}
