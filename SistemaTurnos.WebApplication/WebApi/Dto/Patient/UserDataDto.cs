namespace SistemaTurnos.WebApplication.WebApi.Dto.Patient
{
    public class UserDataDto
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Address { get; set; }

        public string Username { get; set; }

        public string MedicalInsurance { get; set; }

        public int? MedicalInsuranceId { get; set; }

        public string MedicalPlan { get; set; }

        public int? MedicalPlanId { get; set; }

        public int? UserId { get; set; }

        public int? ClientId { get; set; }

        public int? PatientId { get; set; }

        public bool IsClient  {get; set; }

        public bool IsPatient { get; set; }
    }
}
