using System;
using System.ComponentModel.DataAnnotations;

namespace SistemaTurnos.WebApplication.WebApi.Dto.HairdressingAppointment
{
    public class FilterRequestedHairdressingAppointmentDto : BaseDto
    {
        [Required]
        public DateTime Day { get; set; }

        public int? SpecialtyId { get; set; }

        public int? SubspecialtyId { get; set; }
    }
}
