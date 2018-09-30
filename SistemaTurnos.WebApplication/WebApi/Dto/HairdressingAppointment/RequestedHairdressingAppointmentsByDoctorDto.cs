using System.Collections.Generic;

namespace SistemaTurnos.WebApplication.WebApi.Dto.HairdressingAppointment
{
    public class RequestedHairdressingAppointmentsByProfessionalDto : BaseDto
    {
        public int ProfessionalId { get; set; }

        public string ProfessionalFirstName { get; set; }
        
        public string ProfessionalLastName { get; set; }

        public List<HairdressingAppointmentsPerHourDto> RequestedAppointmentsPerHour { get; set; }
    }
}
