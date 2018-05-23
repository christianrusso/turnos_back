using System.Collections.Generic;

namespace SistemaTurnos.WebApplication.WebApi.Dto.Appointment
{
    public class RequestedAppointmentsByDoctorDto : BaseDto
    {
        public int DoctorId { get; set; }

        public string DoctorFirstName { get; set; }
        
        public string DoctorLastName { get; set; }

        public List<AppointmentsPerHourDto> RequestedAppointmentsPerHour { get; set; }
    }
}
