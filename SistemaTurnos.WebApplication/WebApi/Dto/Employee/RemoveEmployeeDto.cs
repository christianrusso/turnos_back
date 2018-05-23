using System.ComponentModel.DataAnnotations;

namespace SistemaTurnos.WebApplication.WebApi.Dto.Employee
{
    public class RemoveEmployeeDto : BaseDto
    {
        [Required]
        public string Email { get; set; }
    }
}
