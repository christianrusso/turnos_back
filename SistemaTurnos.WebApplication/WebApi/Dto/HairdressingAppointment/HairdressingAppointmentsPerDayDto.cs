using System;

namespace SistemaTurnos.WebApplication.WebApi.Dto.HairdressingAppointment
{
    public class HairdressingAppointmentsPerDayDto
    {
        public DateTime Day { get; set; }

        public int AvailableAppointments { get; set; }
    }
}
