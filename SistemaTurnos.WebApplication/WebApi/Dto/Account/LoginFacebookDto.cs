using System.ComponentModel.DataAnnotations;

namespace SistemaTurnos.WebApplication.WebApi.Dto.Account
{
    public class LoginFacebookDto
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string UserId { get; set; }
    }
}
