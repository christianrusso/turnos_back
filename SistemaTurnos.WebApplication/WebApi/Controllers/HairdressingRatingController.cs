using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using SistemaTurnos.WebApplication.Database;
using SistemaTurnos.WebApplication.WebApi.Dto;
using System.Collections.Generic;
using System.Linq;

namespace SistemaTurnos.WebApplication.WebApi.Controllers
{
    [Route("Api/Hairdressing/[controller]/[action]")]
    [Produces("application/json")]
    [EnableCors("AnyOrigin")]
    public class HairdressingRatingController : Controller
    {
        /// <summary>
        ///Obtiene todos los comentarios de una peluqueira dada
        /// </summary>
        [HttpPost]
        public List<string> GetAllUserComments([FromBody] IdDto idDto)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                return dbContext.Hairdressing_Ratings
                    .Where(r => r.UserId == idDto.Id)
                    .Select(r => r.Comment)
                    .ToList();
            }
        }

        /// <summary>
        ///Obiren todos los puntajes de una peluqueria dada.
        /// </summary>
        [HttpPost]
        public List<uint> GetAllUserScores([FromBody] IdDto idDto)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                return dbContext.Hairdressing_Ratings
                    .Where(r => r.UserId == idDto.Id)
                    .Select(r => r.Score)
                    .ToList();
            }
        }

        /// <summary>
        ///Obtiene el puntaje promedio de una peluqueira dada.
        /// </summary>
        [HttpPost]
        public double GetUserRating([FromBody] IdDto idDto)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var ratings = dbContext.Hairdressing_Ratings.Where(r => r.UserId == idDto.Id).ToList();
                return ratings.Any() ? ratings.Average(r => r.Score) : 0;
            }
        }
    }
}
