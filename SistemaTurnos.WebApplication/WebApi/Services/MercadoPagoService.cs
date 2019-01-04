using RestSharp;
using SistemaTurnos.WebApplication.WebApi.Dto.MercadoPago;
using System;
using System.Collections.Generic;

namespace SistemaTurnos.WebApplication.WebApi.Services
{
    public class MercadoPagoService
    {
        public MpPaymentInformationDto GeneratePaymentLink(MpGeneratePaymentRequestDto request)
        {
            try
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
                    },
                    back_urls = new MpBackUrlsDto
                    {
                        success = "https://www.todoreservas.com.ar:4443/#/success",
                        failure = "https://www.todoreservas.com.ar:4443/#/failure",
                        pending = "https://www.todoreservas.com.ar:4443/#/pending",
                    },
                    notification_url = $"https://www.todoreservas.com.ar:4443/Api/Hairdressing/HairdressingAppointment/UpdatePaymentInformation/{request.Id}"
                };

                preferenceRequest.AddJsonBody(preferencePayload);

                var preferenceResponse = client.Execute<MpPreferenceResponseDto>(preferenceRequest);

                return new MpPaymentInformationDto
                {
                    PaymentLink = preferenceResponse.Data.init_point,
                    PreferenceId = preferenceResponse.Data.id
                };
            }
            catch (Exception)
            {
                return null;
            }
        }

        public MpMerchantOrderResponseDto GetMerchantOrder(MpGetMerchantOrderRequestDto request)
        {
            try
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

                var merchantOrderRequest = new RestRequest($"merchant_orders/{request.MerchantOrderId}?access_token={token}", Method.GET, DataFormat.Json);

                var merchantOrderResponse = client.Execute<MpMerchantOrderResponseDto>(merchantOrderRequest);

                return merchantOrderResponse.Data;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
