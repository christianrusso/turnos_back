namespace SistemaTurnos.WebApplication.WebApi.Dto.MercadoPago
{
    public class MpTokenRequestDto
    {
        public string grant_type { get; set; }

        public string client_id { get; set; }

        public string client_secret { get; set; }
    }
}