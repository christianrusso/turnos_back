using System;

namespace SistemaTurnos.WebApplication.WebApi.Dto.Appointment
{
    public class AppointmentDto : BaseDto
    {
        public DateTime Hour { get; set; }

        public string Patient { get; set; }
    }
}
