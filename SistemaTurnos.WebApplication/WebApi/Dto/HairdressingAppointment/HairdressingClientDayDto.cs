using System;
using System.Collections.Generic;

namespace SistemaTurnos.WebApplication.WebApi.Dto.HairdressingAppointment
{
    public class HairdressingClientDayDto
    {
        public DateTime Day { get; set; }

        public List<PatientHairdressingAppointmentInformationDto> Appointments { get; set; }
    }
}
