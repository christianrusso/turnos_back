namespace SistemaTurnos.WebApplication.WebApi.Dto.Clinic
{
    public class ClinicDto : BaseDto
    {
        public int ClinicId { get; set; }

        public string Address { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public double DistanceToUser { get; set; }
    }
}
