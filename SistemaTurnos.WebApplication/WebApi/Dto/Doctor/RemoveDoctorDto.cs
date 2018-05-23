using System.ComponentModel.DataAnnotations;

namespace SistemaTurnos.WebApplication.WebApi.Dto.Doctor
{
    public class RemoveDoctorDto : BaseDto
    {
        [Required]
        public int Id { get; set; }
    }
}
