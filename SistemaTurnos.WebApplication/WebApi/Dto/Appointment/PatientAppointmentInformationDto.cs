using System;
using SistemaTurnos.Database.Enums;

namespace SistemaTurnos.WebApplication.WebApi.Dto.Appointment
{
    public class PatientAppointmentInformationDto
    {
        public int ClinicId { get; set; }

        public string Clinic { get; set; }

        public string Doctor { get; set; }

        public string Specialty { get; set; }

        public string Subspecialty { get; set; }

        public DateTime DateTime { get; set; }

        public string Logo { get; set; }

        public int Id { get; set; }

        public AppointmentStateEnum State { get; set; }
    }
}
