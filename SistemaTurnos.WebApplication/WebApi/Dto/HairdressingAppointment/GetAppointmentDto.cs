using System;
using System.ComponentModel.DataAnnotations;

namespace SistemaTurnos.WebApplication.WebApi.Dto.HairdressingAppointment
{
    public class GetHairdressingAppointmentDto : BaseDto
    {
        public int? HairdressingId { get; set; }
        
        [Required]
        public DateTime Day { get; set; }

        [Required]
        public int ProfessionalId { get; set; }

        [Required]
        public int SubspecialtyId { get; set; }
    }
}
