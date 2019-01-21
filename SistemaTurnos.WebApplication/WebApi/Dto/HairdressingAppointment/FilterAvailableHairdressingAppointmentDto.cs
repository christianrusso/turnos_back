using System;
using System.ComponentModel.DataAnnotations;

namespace SistemaTurnos.WebApplication.WebApi.Dto.HairdressingAppointment
{
    public class FilterAvailableHairdressingAppointmentDto
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int? HairdressingId { get; set; }

        public int? ProfessionalId { get; set; }

        [Required]
        public int SubSpecialtyId { get; set; }
    }
}
