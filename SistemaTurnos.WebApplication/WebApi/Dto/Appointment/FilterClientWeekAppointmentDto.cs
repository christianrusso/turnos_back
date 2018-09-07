using System;

namespace SistemaTurnos.WebApplication.WebApi.Dto.Appointment
{
    public class FilterClientWeekAppointmentDto
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
