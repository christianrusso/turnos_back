using SistemaTurnos.Database.Enums;
using System.ComponentModel.DataAnnotations;

namespace SistemaTurnos.WebApplication.WebApi.Dto.Hairdressing
{
    public class HairdressingGeoLocationDto : BaseDto
    {
        [Required]
        public double Latitude { get; set; }

        [Required]
        public double Longitude { get; set; }

        [Required]
        public double RadiusInMeters { get; set; }

        [Required]
        public BusinessType BusinessType { get; set; }
    }
}
