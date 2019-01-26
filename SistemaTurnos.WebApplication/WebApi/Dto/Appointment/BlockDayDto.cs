using System;
using System.ComponentModel.DataAnnotations;

namespace SistemaTurnos.WebApplication.WebApi.Dto.Appointment
{
    public class BlockDayDto
    {
        [Required]
        public DateTime Day { get; set; }

        [Required]
        public int Id { get; set; }

        [Required]
        public int SubspecialtyId { get; set; }
    }
}
