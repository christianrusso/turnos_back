namespace SistemaTurnos.WebApplication.WebApi.Dto.Appointment
{
    public class AppointmentsPerSpecialtyDto : BaseDto
    {
        public int SpecialtyId { get; set; }

        public string SpecialtyDescription { get; set; }

        public int Appointments { get; set; }
    }
}