using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using SistemaTurnos.WebApplication.WebApi.Dto.Email;
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
    }
}
