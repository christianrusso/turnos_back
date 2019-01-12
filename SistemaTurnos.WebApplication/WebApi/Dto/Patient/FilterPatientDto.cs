namespace SistemaTurnos.WebApplication.WebApi.Dto.Patient
{
    public class FilterPatientDto : BaseDto
    {
        public int? MedicalInsuranceId { get; set; }

        public string Text { get; set; }
    }
}
