namespace SistemaTurnos.WebApplication.WebApi.Dto.MercadoPago
{
    public class MpTokenResponseDto
    {
        public string access_token { get; set; }

        public string refresh_token { get; set; }

        public bool live_mode { get; set; }

        public string user_id { get; set; }

        public string token_type { get; set; }

        public string expires_in { get; set; }

        public string scope { get; set; }
    }
}