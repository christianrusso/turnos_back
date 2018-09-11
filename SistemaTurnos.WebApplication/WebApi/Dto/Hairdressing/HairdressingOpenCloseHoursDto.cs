using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SistemaTurnos.WebApplication.WebApi.Dto.Common;

namespace SistemaTurnos.WebApplication.WebApi.Dto.Hairdressing
{
    public class HairdressingOpenCloseHoursDto
    {
        [Required]
        public int HairdressingId { get; set; }

        [Required]
        public List<OpenCloseHoursDto> OpenCloseHours { get; set; }
    }
}
