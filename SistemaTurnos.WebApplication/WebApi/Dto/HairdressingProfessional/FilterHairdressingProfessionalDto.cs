namespace SistemaTurnos.WebApplication.WebApi.Dto.HairdressingProfessional
{
    public class FilterHairdressingProfessionalDto : BaseDto
    {
        public int? Id { get; set; }

        public int? ClinicId { get; set; }

        public string FullName { get; set; }

        public int? SpecialtyId { get; set; }

        public int? SubspecialtyId { get; set; }
    }
}
