using System.ComponentModel.DataAnnotations;

namespace SistemaTurnos.WebApplication.WebApi.Dto.Doctor
{
    public class EnableDisableDoctorDto : BaseDto
    {
        [Required]
        public int Id { get; set; }
    }
}
