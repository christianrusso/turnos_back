using System.ComponentModel.DataAnnotations;

namespace SistemaTurnos.WebApplication.WebApi.Dto.Subspecialty
{
    public class EditSubspecialtyDto : BaseDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 4)]
        public string Description { get; set; }

        [Required]
        public uint ConsultationLength { get; set; }

        [Required]
        public int SpecialtyId { get; set; }
    }
}
