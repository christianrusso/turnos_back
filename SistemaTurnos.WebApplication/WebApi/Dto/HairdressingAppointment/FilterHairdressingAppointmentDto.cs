using System;

namespace SistemaTurnos.WebApplication.WebApi.Dto.HairdressingAppointment
{
    public class FilterHairdressingAppointmentDto : BaseDto
    {
        public int? Id { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int ProfessionalId { get; set; }

        public int? SpecialtyId { get; set; }

        public int? SubSpecialtyId { get; set; }
    }
}
