using System;
using System.ComponentModel.DataAnnotations;

namespace SistemaTurnos.WebApplication.WebApi.Dto.Appointment
{
    public class GetAppointmentDto : BaseDto
    {
        public int? ClinicId { get; set; }
        
        [Required]
        public DateTime Day { get; set; }

        [Required]
        public int DoctorId { get; set; }
    }
}
