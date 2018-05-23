using System.ComponentModel.DataAnnotations;

namespace SistemaTurnos.WebApplication.WebApi.Dto.Appointment
{
    public class CompleteAppointmentDto : BaseDto
    {
        [Required]
        public int Id { get; set; }

        [Range(0, 10)]
        public uint Score { get; set; }

        public string Comment { get; set; }
    }
}
