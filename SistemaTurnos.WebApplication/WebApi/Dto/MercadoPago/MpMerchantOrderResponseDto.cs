namespace SistemaTurnos.WebApplication.WebApi.Dto.MercadoPago
{
    public class MpMerchantOrderResponseDto
    {
        public string id { get; set; }

        public string preference_id { get; set; }

        public string status { get; set; }

        public decimal paid_amount { get; set; }

        public decimal refunded_amount { get; set; }

        public bool cancelled { get; set; }

        public decimal total_amount { get; set; }
    }
}
