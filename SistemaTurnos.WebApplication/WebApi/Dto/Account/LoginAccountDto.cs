using SistemaTurnos.Database.Enums;
using SistemaTurnos.Database.HairdressingModel;
using System.ComponentModel.DataAnnotations;

namespace SistemaTurnos.WebApplication.WebApi.Dto.Account
{
    public class LoginAccountDto : BaseDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
