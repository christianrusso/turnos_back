namespace SistemaTurnos.WebApplication.WebApi.Dto.HairdressingPatient
{
    public class HairdressingPatientDto
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Address { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public int UserId { get; set; }

        public int ClientId { get; set; }

        public int ReservedAppointments { get; set; }

        public int ConcretedAppointments { get; set; }
    }
}
