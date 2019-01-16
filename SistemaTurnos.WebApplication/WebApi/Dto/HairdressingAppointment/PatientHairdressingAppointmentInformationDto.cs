using System;
using SistemaTurnos.Database.Enums;

namespace SistemaTurnos.WebApplication.WebApi.Dto.HairdressingAppointment
{
    public class PatientHairdressingAppointmentInformationDto
    {
        public int HairdressingId { get; set; }

        public string Hairdressing { get; set; }

        public string Professional { get; set; }

        public string Specialty { get; set; }

        public string Logo { get; set; }

        public string Subspecialty { get; set; }

        public DateTime DateTime { get; set; }

        public AppointmentStateEnum State { get; set; }
    }
}
