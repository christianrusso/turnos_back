using System;
using System.ComponentModel.DataAnnotations;

namespace SistemaTurnos.WebApplication.WebApi.Dto.Appointment
{
    public class FilterRequestedAppointmentDto : BaseDto
    {
        [Required]
        public DateTime Day { get; set; }

        public int? SpecialtyId { get; set; }

        public int? SubspecialtyId { get; set; }
    }
}
