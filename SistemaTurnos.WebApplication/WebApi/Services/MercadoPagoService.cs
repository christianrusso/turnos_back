using RestSharp;
using SistemaTurnos.WebApplication.WebApi.Dto.MercadoPago;
using System.Collections.Generic;

namespace SistemaTurnos.WebApplication.WebApi.Services
{
    public class MercadoPagoService
    {
        public string GeneratePaymentLink(MpRequestDto request)
        {
            var client = new RestClient("https://api.mercadopago.com");

            var tokenRequest = new RestRequest("oauth/token", Method.POST, DataFormat.Json);

            var tokenPayload = new MpTokenRequestDto
            {
                grant_type = "client_credentials",
                client_id = request.ClientId,
                client_secret = request.ClientSecret
            };

            tokenRequest.AddJsonBody(tokenPayload);

            var response = client.Execute<MpTokenResponseDto>(tokenRequest);

            var token = response.Data.access_token;

            var preferenceRequest = new RestRequest($"checkout/preferences?access_token={token}", Method.POST, DataFormat.Json);

            var preferencePayload = new MpPreferenceRequestDto
            {
                items = new List<MpItemDto>
                {
                    new MpItemDto
                    {
                        title = request.Title,
                        currency_id = "ARS",
                        quantity = 1,
                        unit_price = request.Price
                    }
                },
                payer = new MpPayerDto
                {
                    email = request.BuyerEmail
                }
            };

            preferenceRequest.AddJsonBody(preferencePayload);

            var preferenceResponse = client.Execute<MpPreferenceResponseDto>(preferenceRequest);

            return preferenceResponse.Data.sandbox_init_point;
        }
    }
}
