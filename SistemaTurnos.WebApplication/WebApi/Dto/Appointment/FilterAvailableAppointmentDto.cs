using System;

namespace SistemaTurnos.WebApplication.WebApi.Dto.Appointment
{
    public class FilterAvailableAppointmentDto
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int? ClinicId { get; set; }

        public int? DoctorId { get; set; }

        public int? SpecialtyId { get; set; }

        public int? SubSpecialtyId { get; set; }
    }
}
