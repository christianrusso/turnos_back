using System;

namespace SistemaTurnos.WebApplication.WebApi.Dto.HairdressingAppointment
{
    public class HairdressingAppointmentDto : BaseDto
    {
        public int Id { get; set; }

        public DateTime Hour { get; set; }

        public string Patient { get; set; }

        public int State { get; set; }
    }
}
