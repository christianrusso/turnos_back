using System.ComponentModel.DataAnnotations;

namespace SistemaTurnos.WebApplication.WebApi.Dto
{
    public class SelectOptionDto : BaseDto
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public string Text { get; set; }
    }
}
