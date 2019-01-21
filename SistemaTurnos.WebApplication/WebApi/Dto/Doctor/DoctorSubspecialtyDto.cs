using System.ComponentModel.DataAnnotations;

namespace SistemaTurnos.WebApplication.WebApi.Dto.Doctor
{
    public class DoctorSubspecialtyDto
    {
        [Required]
        public int SubspecialtyId { get; set; }

        [Required]
        public uint ConsultationLength { get; set; }
    }
}
