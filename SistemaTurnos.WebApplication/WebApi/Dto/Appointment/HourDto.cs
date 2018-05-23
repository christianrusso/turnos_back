using System;
using System.Collections.Generic;

namespace SistemaTurnos.WebApplication.WebApi.Dto.Appointment
{
    public class HourDto : BaseDto
    {
        public DateTime Hour { get; set; }

        public int TotalAppointments { get; set; }

        public List<AppointmentsPerSpecialtyDto> AppointmentsPerSpecialty { get; set; }
    }
}