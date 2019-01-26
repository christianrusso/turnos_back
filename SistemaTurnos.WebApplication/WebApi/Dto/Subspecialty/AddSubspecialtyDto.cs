using System.ComponentModel.DataAnnotations;

namespace SistemaTurnos.WebApplication.WebApi.Dto.Subspecialty
{
    public class AddSubspecialtyDto : BaseDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public uint ConsultationLength { get; set; }

        [Required]
        public string Indications { get; set; }

        [Required]
        public int SpecialtyId { get; set; }
    }
}
