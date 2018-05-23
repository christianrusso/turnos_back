using System.ComponentModel.DataAnnotations;

namespace SistemaTurnos.WebApplication.WebApi.Dto.Patient
{
    public class RemovePatientDto : BaseDto
    {
        [Required]
        public int Id { get; set; }
    }
}
