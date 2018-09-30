using System.Collections.Generic;

namespace SistemaTurnos.WebApplication.WebApi.Dto.HairdressingAppointment
{
    public class HairdressingAppointmentsPerHourDto : BaseDto
    {
        public int Hour { get; set; }

        public List<HairdressingAppointmentDto> Appointments { get; set; }
    }
}
