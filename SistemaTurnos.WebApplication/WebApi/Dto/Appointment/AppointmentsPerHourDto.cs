using System.Collections.Generic;

namespace SistemaTurnos.WebApplication.WebApi.Dto.Appointment
{
    public class AppointmentsPerHourDto : BaseDto
    {
        public int Hour { get; set; }

        public List<AppointmentDto> Appointments { get; set; }
    }
}
