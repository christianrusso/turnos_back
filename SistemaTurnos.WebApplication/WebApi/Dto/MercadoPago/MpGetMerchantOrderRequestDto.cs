namespace SistemaTurnos.WebApplication.WebApi.Dto.MercadoPago
{
    public class MpGetMerchantOrderRequestDto
    {
        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string MerchantOrderId { get; set; }
    }
}
