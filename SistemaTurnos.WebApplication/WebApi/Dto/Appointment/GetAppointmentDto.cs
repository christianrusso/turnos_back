using System;
using System.ComponentModel.DataAnnotations;

namespace SistemaTurnos.WebApplication.WebApi.Dto.Appointment
{
    public class GetAppointmentDto : BaseDto
    {
        [Required]
        public DateTime Day { get; set; }

        [Required]
        public int DoctorId { get; set; }
    }
}
