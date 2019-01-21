using System;
using System.ComponentModel.DataAnnotations;

namespace SistemaTurnos.WebApplication.WebApi.Dto.Appointment
{
    public class RequestAppointmentForPatientDto : BaseDto
    {
        [Required]
        public int PatientId { get; set; }

        [Required]
        public DateTime Day { get; set; }

        [Required]
        public DateTime Time { get; set; }

        [Required]
        public int DoctorId { get; set; }

        [Required]
        public int SubspecialtyId { get; set; }
    }
}
