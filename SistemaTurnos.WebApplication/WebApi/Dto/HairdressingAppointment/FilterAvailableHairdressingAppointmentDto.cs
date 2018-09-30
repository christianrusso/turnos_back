using System;

namespace SistemaTurnos.WebApplication.WebApi.Dto.HairdressingAppointment
{
    public class FilterAvailableHairdressingAppointmentDto
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int? HairdressingId { get; set; }

        public int? ProfessionalId { get; set; }

        public int? SpecialtyId { get; set; }

        public int? SubSpecialtyId { get; set; }
    }
}
