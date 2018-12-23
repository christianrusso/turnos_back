using System;
using System.ComponentModel.DataAnnotations;

namespace SistemaTurnos.WebApplication.WebApi.Dto.Appointment
{
    public class RequestAppointmentByPatientDto : BaseDto
    {
        [Required]
        public int ClinicId { get; set; }

        [Required]
        public DateTime Day { get; set; }

        [Required]
        public DateTime Time { get; set; }

        [Required]
        public int DoctorId { get; set; }

        [Required]
        [Range(1, 3)]
        public int Source { get; set; }
    }
}
