using System.ComponentModel.DataAnnotations;

namespace SistemaTurnos.WebApplication.WebApi.Dto.HairdressingPatient
{
    public class AddHairdressingPatientDto : BaseDto
    {
        [Required]
        public int ClientId { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 4)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string LastName { get; set; }

        [Required]
        [StringLength(500, MinimumLength = 2)]
        public string Address { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 4)]
        public string Username { get; set; }

    }
}
