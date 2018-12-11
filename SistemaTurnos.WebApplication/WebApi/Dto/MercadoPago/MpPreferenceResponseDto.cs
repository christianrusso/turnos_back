namespace SistemaTurnos.WebApplication.WebApi.Dto.MercadoPago
{
    public class MpPreferenceResponseDto
    {
        public string id { get; set; }

        public string init_point { get; set; }

        public string sandbox_init_point { get; set; }

        public MpBackUrlsDto back_urls { get; set; }

        public string notification_url { get; set; }
    }

    public class MpBackUrlsDto
    {
        public string success { get; set; }

        public string pending { get; set; }

        public string failure { get; set; }
    }
}
