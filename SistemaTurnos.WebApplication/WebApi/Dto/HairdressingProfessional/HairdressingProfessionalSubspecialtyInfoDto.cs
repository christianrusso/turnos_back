namespace SistemaTurnos.WebApplication.WebApi.Dto.HairdressingProfessional
{
    public class HairdressingProfessionalSubspecialtyInfoDto
    {
        public int SpecialtyId { get; set; }

        public string SpecialtyDescription { get; set; }

        public int SubspecialtyId { get; set; }

        public string SubspecialtyDescription { get; set; }

        public uint ConsultationLength { get; set; }
    }
}
