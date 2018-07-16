using SistemaTurnos.WebApplication.WebApi.Dto.Rating;
using System.Collections.Generic;

namespace SistemaTurnos.WebApplication.WebApi.Dto.Clinic
{
    public class FullClinicDto :BaseDto
    {
        public int ClinicId { get; set; }

        public string Name { get; set; }
        
        public string Description { get; set; }

        public string City { get; set; }

        public string Address { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public double DistanceToUser { get; set; }

        public List<string> Specialties { get; set; }

        public List<string> Subspecialties { get; set; }

        public List<string> MedicalInsurances { get; set; }

        public double Score { get; set; }

        public int ScoreQuantity { get; set; }

        public string Logo {get; set;}

        public List<RatingDto> Ratings { get; set; }
    }
}
