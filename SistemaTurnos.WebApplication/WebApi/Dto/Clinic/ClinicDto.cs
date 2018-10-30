namespace SistemaTurnos.WebApplication.WebApi.Dto.Clinic
{
    public class ClinicDto : BaseDto
    {
        public int ClinicId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string City { get; set; }

        public string Address { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string Logo { get; set; }

        public double DistanceToUser { get; set; }

        public double Score { get; set; }
    }
}
