namespace SistemaTurnos.WebApplication.WebApi.Dto.Doctor
{
    public class DoctorSubspecialtyInfoDto
    {
        public int SpecialtyId { get; set; }

        public string SpecialtyDescription { get; set; }

        public int SubspecialtyId { get; set; }

        public string SubspecialtyDescription { get; set; }

        public uint ConsultationLength { get; set; }
    }
}
