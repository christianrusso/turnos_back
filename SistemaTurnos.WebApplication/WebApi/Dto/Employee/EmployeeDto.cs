using System.ComponentModel.DataAnnotations;

namespace SistemaTurnos.WebApplication.WebApi.Dto.Employee
{
    public class EmployeeDto : BaseDto
    {
        [Required]
        public string Email { get; set; }
    }
}
