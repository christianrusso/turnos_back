namespace SistemaTurnos.WebApplication.WebApi.Dto.Client
{
    public class ClientProfileDto : BaseDto
    {
        public string Username { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Dni { get; set; }

        public string Email { get; set; }

        public string Address { get; set; }

        public string PhoneNumber { get; set; }

        public string Logo { get; set; }
    }
}
