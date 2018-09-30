namespace SistemaTurnos.WebApplication.WebApi.Dto.HairdressingAppointment
{
    public class HairdressingAppointmentsPerSpecialtyDto : BaseDto
    {
        public int SpecialtyId { get; set; }

        public string SpecialtyDescription { get; set; }

        public int Appointments { get; set; }
    }
}