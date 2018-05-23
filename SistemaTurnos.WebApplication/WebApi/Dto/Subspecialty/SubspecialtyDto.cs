namespace SistemaTurnos.WebApplication.WebApi.Dto.Subspecialty
{
    public class SubspecialtyDto : BaseDto
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public uint ConsultationLength { get; set; }

        public int SpecialtyId { get; set; }

        public string SpecialtyDescription { get; set; }
    }
}
