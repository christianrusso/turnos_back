namespace SistemaTurnos.WebApplication.Database.Model
{
    public class PatientDto
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string MedicalInsurance { get; set; }

        public int MedicalInsuranceId { get; set; }

        public string MedicalPlan { get; set; }

        public int MedicalPlanId { get; set; }

        public string Address { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public int UserId { get; set; }

        public int ClientId { get; set; }

        public int ReservedAppointments { get; set; }

        public int ConcretedAppointments { get; set; }
    }
}
