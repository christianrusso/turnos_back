using SistemaTurnos.WebApplication.WebApi.Dto.Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SistemaTurnos.WebApplication.WebApi.Dto.HairdressingProfessional
{
    public class AddHairdressingProfessionalDto : BaseDto
    {
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string LastName { get; set; }

        [StringLength(50)]
        public string Email { get; set; }

        [StringLength(50)]
        public string PhoneNumber { get; set; }

        [Required]
        public List<HairdressingProfessionalSubspecialtyDto> Subspecialties { get; set; }

        [Required]
        public List<WorkingHoursDto> WorkingHours { get; set; }
    }
}
