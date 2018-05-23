using System.ComponentModel.DataAnnotations;

namespace SistemaTurnos.WebApplication.WebApi.Dto.Client
{
    public class RemoveClientDto : BaseDto
    {
        [Required]
        public string Email { get; set; }
    }
}
