using MercadoPago;
using MercadoPago.Resources;
using MercadoPago.DataStructures.Preference;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using SistemaTurnos.WebApplication.WebApi.Dto.Email;
using SistemaTurnos.WebApplication.WebApi.Services;
using System.Collections.Generic;
using MercadoPago.Common;

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
        public void Pay()
        {
            SDK.ClientId = "2128552166781000";
            SDK.ClientSecret = "xt23Yx9BO3wqXO26aHWlzxvTuw7vFo6G";

            // Create a preference object
            Preference preference = new Preference();
            preference.Items.Add(
              new Item()
              {
                  Id = "1234",
                  Title = "Enormous Iron Shirt",
                  Quantity = 7,
                  CurrencyId = CurrencyId.ARS,
                  UnitPrice = 10
              }
            );

            //preference.BackUrls = new BackUrls
            //{
            //    Success = 
            //}

            // Setting a payer object as value for Payer property
            preference.Payer = new Payer()
            {
                Email = "christian.russo8@gmail.com"
            };

            // Save and posting preference
            preference.Save();

            var asd = preference.InitPoint;
            asd = "ESAA";
        }
    }
}
