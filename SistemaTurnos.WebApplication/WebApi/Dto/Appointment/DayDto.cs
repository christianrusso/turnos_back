using System;
using System.Collections.Generic;

namespace SistemaTurnos.WebApplication.WebApi.Dto.Appointment
{
    public class DayDto : BaseDto
    {
        public DateTime Day { get; set; }

        public List<HourDto> Hours { get; set; }
    }
}
