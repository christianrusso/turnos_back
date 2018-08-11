using System.ComponentModel.DataAnnotations;

namespace SistemaTurnos.WebApplication.WebApi.Dto
{
    public class OptionalIdDto : BaseDto
    {
        [Required]
        public int? Id { get; set; }
    }
}
