using System.ComponentModel.DataAnnotations;

namespace SistemaTurnos.WebApplication.WebApi.Dto.Subspecialty
{
    public class EditSubspecialtyDto : BaseDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public uint ConsultationLength { get; set; }
    }
}
