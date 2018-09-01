namespace SistemaTurnos.WebApplication.WebApi.Dto.Doctor
{
    public class FilterDoctorDto : BaseDto
    {
        public int? Id { get; set; }

        public int? ClinicId { get; set; }

        public string FullName { get; set; }

        public int? SpecialtyId { get; set; }

        public int? SubspecialtyId { get; set; }
    }
}
