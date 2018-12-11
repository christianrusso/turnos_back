namespace SistemaTurnos.WebApplication.WebApi.Dto.MercadoPago
{
    public class MpRequestDto
    {
        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string Title { get; set; }

        public decimal Price { get; set; }

        public string BuyerEmail { get; set; }

        public string SuccessBackUrl { get; set; }

        public string FailureBackUrl { get; set; }

        public string PendingBackUrl { get; set; }

        public string NotificationUrl { get; set; }
    }
}
