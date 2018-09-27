using System.ComponentModel.DataAnnotations;

namespace SistemaTurnos.WebApplication.WebApi.Dto.HairdressingAppointment
{
    public class CompleteHairdressingAppointmentDto : BaseDto
    {
        [Required]
        public int Id { get; set; }

        [Range(0, 10)]
        public uint Score { get; set; }

        public string Comment { get; set; }
    }
}
