namespace SistemaTurnos.WebApplication.WebApi.Dto.MercadoPago
{
    public class MpGeneratePaymentRequestDto
    {
        public int Id { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string Title { get; set; }

        public decimal Price { get; set; }

        public string BuyerEmail { get; set; }
    }
}
