using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using SistemaTurnos.WebApplication.WebApi.Dto.Email;
using SistemaTurnos.WebApplication.WebApi.Dto.MercadoPago;
using SistemaTurnos.WebApplication.WebApi.Services;
using System.Collections.Generic;

namespace SistemaTurnos.WebApplication.WebApi.Controllers
{
    [Route("Api/[controller]/[action]")]
    [Produces("application/json")]
    [EnableCors("AnyOrigin")]
    public class TestController : Controller
    {
        [HttpPost]
        public void SendEmail()
        {
            new EmailService().Send(new EmailDto
            {
                From = "no-reply@tuturno.com.ar",
                To = new List<string>() { "fernando.gmz12@gmail.com", "christian.russo8@gmail.com" },
                Subject = "Sistema de turnos",
                Message = "<strong>MENSAJE DE PRUEBA CON ESTILOS EN HTML</strong>"
            });
        }

        [HttpPost]
        public MpPaymentInformationDto Pay()
        {
            return new MercadoPagoService().GeneratePaymentLink(new MpGeneratePaymentRequestDto
            {
                SellerId = 1,
                ClientId = "2128552166781000",
                ClientSecret = "xt23Yx9BO3wqXO26aHWlzxvTuw7vFo6G",
                Title = "Item de prueba",
                Price = 10,
                BuyerEmail = "christian.russo8@gmail.com",
            });
        }
    }
}
