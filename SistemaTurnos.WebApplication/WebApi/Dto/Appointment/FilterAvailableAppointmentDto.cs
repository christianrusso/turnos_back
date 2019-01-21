using System;
using System.ComponentModel.DataAnnotations;

namespace SistemaTurnos.WebApplication.WebApi.Dto.Appointment
{
    public class FilterAvailableAppointmentDto
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int? ClinicId { get; set; }

        public int? DoctorId { get; set; }

        [Required]
        public int SubSpecialtyId { get; set; }
    }
}
