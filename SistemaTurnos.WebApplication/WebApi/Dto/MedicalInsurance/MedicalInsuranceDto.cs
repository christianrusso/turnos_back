using System.Collections.Generic;

namespace SistemaTurnos.WebApplication.WebApi.Dto.MedicalInsurance
{
    public class MedicalInsuranceDto
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public List<MedicalPlanDto> MedicalPlans { get; set; }

    }
}
