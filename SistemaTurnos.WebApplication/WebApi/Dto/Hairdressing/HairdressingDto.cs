namespace SistemaTurnos.WebApplication.WebApi.Dto.Hairdressing
{
    public class HairdressingDto : BaseDto
    {
        public int HairdressingId { get; set; }

        public string Address { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public double DistanceToUser { get; set; }
    }
}
