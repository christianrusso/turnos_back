using System;
using System.Collections.Generic;

namespace SistemaTurnos.WebApplication.WebApi.Dto.HairdressingAppointment
{
    public class HourDto : BaseDto
    {
        public DateTime Hour { get; set; }

        public int TotalAppointments { get; set; }

        public List<HairdressingAppointmentsPerSpecialtyDto> AppointmentsPerSpecialty { get; set; }
    }
}