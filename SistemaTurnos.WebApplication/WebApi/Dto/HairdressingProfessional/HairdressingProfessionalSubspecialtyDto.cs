using System.ComponentModel.DataAnnotations;

namespace SistemaTurnos.WebApplication.WebApi.Dto.HairdressingProfessional
{
    public class HairdressingProfessionalSubspecialtyDto
    {
        [Required]
        public int SubspecialtyId { get; set; }

        [Required]
        public uint ConsultationLength { get; set; }
    }
}
