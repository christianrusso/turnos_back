using System;
using System.Collections.Generic;

namespace SistemaTurnos.WebApplication.WebApi.Dto.Appointment
{
    public class ClientDayDto
    {
        public DateTime Day { get; set; }

        public List<PatientAppointmentInformationDto> Appointments { get; set; }
    }
}
