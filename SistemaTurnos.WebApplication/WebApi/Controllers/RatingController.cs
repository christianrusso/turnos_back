using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using SistemaTurnos.Database;
using SistemaTurnos.WebApplication.WebApi.Dto;
using System.Collections.Generic;
using System.Linq;

namespace SistemaTurnos.WebApplication.WebApi.Controllers
{
    [Route("Api/[controller]/[action]")]
    [Produces("application/json")]
    [EnableCors("AnyOrigin")]
    public class RatingController : Controller
    {

        /// <summary>
        ///Devuelve todos los comentarios de una clinica dada.
        /// </summary>
        [HttpPost]
        public List<string> GetAllUserComments([FromBody] IdDto idDto)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                return dbContext.Clinic_Ratings
                    .Where(r => r.Appointment.Patient.Client.UserId == idDto.Id)
                    .Select(r => r.Comment)
                    .ToList();
            }
        }

        /// <summary>
        ///Devuelve todos los puntajes de una clinica dada.
        /// </summary>
        [HttpPost]
        public List<uint> GetAllUserScores([FromBody] IdDto idDto)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                return dbContext.Clinic_Ratings
                    .Where(r => r.Appointment.Patient.Client.UserId == idDto.Id)
                    .Select(r => r.Score)
                    .ToList();
            }
        }

        /// <summary>
        ///Devuelve el promedio del puntaje de una clinica dada.
        /// </summary>
        [HttpPost]
        public double GetUserRating([FromBody] IdDto idDto)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var ratings = dbContext.Clinic_Ratings.Where(r => r.Appointment.Patient.Client.UserId == idDto.Id).ToList();
                return ratings.Any() ? ratings.Average(r => r.Score) : 0;
            }
        }
    }
}
