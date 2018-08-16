using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SistemaTurnos.WebApplication.WebApi.Dto.Clinic
{
    public class ClinicOpenCloseHoursDto
    {
        [Required]
        public int ClinicId { get; set; }

        [Required]
        public List<OpenCloseHoursDto> OpenCloseHours { get; set; }
    }
}
