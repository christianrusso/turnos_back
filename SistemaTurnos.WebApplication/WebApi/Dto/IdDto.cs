using System.ComponentModel.DataAnnotations;

namespace SistemaTurnos.WebApplication.WebApi.Dto
{
    public class IdDto : BaseDto
    {
        [Required]
        public int Id { get; set; }
    }
}
