using System.ComponentModel.DataAnnotations;

namespace SistemaTurnos.WebApplication.WebApi.Dto.Clinic
{
    public class UpdateClinicDto
    {
        [Required]
        public int ClinicId { get; set; }

        public string Address { get; set; }

        public string Description { get; set; }

        public int? CityId { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public string Logo { get; set; }
    }
}
