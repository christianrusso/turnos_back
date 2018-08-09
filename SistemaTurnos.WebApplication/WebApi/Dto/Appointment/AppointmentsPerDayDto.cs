using System;

namespace SistemaTurnos.WebApplication.WebApi.Dto.Appointment
{
    public class AppointmentsPerDayDto
    {
        public DateTime Day { get; set; }

        public int AvailableAppointments { get; set; }
    }
}
