using System;

namespace SistemaTurnos.WebApplication.WebApi.Dto.Appointment
{
    public class AppointmentDto : BaseDto
    {
        public int Id { get; set; }

        public DateTime Hour { get; set; }

        public string Patient { get; set; }

        public int State { get; set; }
    }
}
