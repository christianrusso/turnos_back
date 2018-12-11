using System.Collections.Generic;

namespace SistemaTurnos.WebApplication.WebApi.Dto.MercadoPago
{
    public class MpPreferenceRequestDto
    {
        public List<MpItemDto> items { get; set; }

        public MpPayerDto payer { get; set; }

        public MpBackUrlsDto back_urls { get; set; }

        public string notification_url { get; set; }
    }

    public class MpPayerDto
    {
        public string email { get; set; }
    }

    public class MpItemDto
    {
        public string title { get; set; }

        public uint quantity { get; set; }

        public string currency_id { get; set; }

        public decimal unit_price { get; set; }
    }
}
