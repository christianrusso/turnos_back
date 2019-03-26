namespace SistemaTurnos.WebApplication.WebApi.Dto.Client
{
    public class ClientDto : BaseDto
    {
        public int Id { get; set; }

        public string Email { get; set; }

        public int ReservedAppointments { get; set; }

        public string FirstName { get; set; }
        
        public string LastName { get; set; }
        
        public string Address { get; set; }
        
        public string PhoneNumber { get; set; }
    }
}
