using System.ComponentModel.DataAnnotations;

namespace SistemaTurnos.WebApplication.WebApi.Dto.Patient
{
    public class AddPatientDto : BaseDto
    {
        [Required]
        public int ClientId { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 4)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 4)]
        public string LastName { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 4)]
        public string Address { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 4)]
        public string PhoneNumber { get; set; }

        [StringLength(50)]
        public string Email { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 4)]
        public string Dni { get; set; }

        public int MedicalPlanId { get; set; }
    }
}
