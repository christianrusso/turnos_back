using System.Collections.Generic;

namespace SistemaTurnos.WebApplication.WebApi.Dto.Clinic
{
    public class FilterClinicDto : BaseDto
    {
        public List<int> Specialties { get; set; }
    
        public List<int> Subspecialties { get; set; }

        public List<int> MedicalInsurances { get; set; }

        public List<int> MedicalPlans { get; set; }

        public List<string> Cities { get; set; }

        public GeoLocationDto Location { get; set; }

        public double? Score { get; set; }

        public int? ScoreQuantity { get; set; }
    }
}
