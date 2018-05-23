namespace SistemaTurnos.WebApplication.WebApi.Dto.Client
{
    public class ClientDto : BaseDto
    {
        public int Id { get; set; }

        public string Email { get; set; }

        public int ReservedAppointments { get; set; }
    }
}
