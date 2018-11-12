using System.ComponentModel.DataAnnotations;

namespace SistemaTurnos.WebApplication.WebApi.Dto
{
    public class IdAndUserDto : BaseDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }
    }
}
