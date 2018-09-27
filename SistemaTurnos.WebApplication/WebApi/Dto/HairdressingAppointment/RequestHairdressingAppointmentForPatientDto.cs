using System;
using System.ComponentModel.DataAnnotations;

namespace SistemaTurnos.WebApplication.WebApi.Dto.HairdressingAppointment
{
    public class RequestHairdressingAppointmentForPatientDto : BaseDto
    {
        [Required]
        public int PatientId { get; set; }

        [Required]
        public DateTime Day { get; set; }

        [Required]
        public DateTime Time { get; set; }

        [Required]
        public int ProfessionalId { get; set; }
    }
}
