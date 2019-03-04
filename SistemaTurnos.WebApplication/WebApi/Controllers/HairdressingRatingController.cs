using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using SistemaTurnos.Database;
using SistemaTurnos.WebApplication.WebApi.Dto;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            var watch = Stopwatch.StartNew();

            using (var dbContext = new ApplicationDbContext())
            {
                var res = dbContext.Hairdressing_Ratings
                    .Where(r => r.UserId == idDto.Id)
                    .Select(r => r.Comment)
                    .ToList();

                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                Console.WriteLine("HairdressingRating/GetAllUserComments milisegundos: " + elapsedMs);

                return res;
            }
        }

        /// <summary>
        ///Obiren todos los puntajes de una peluqueria dada.
        /// </summary>
        [HttpPost]
        public List<uint> GetAllUserScores([FromBody] IdDto idDto)
        {
            var watch = Stopwatch.StartNew();

            using (var dbContext = new ApplicationDbContext())
            {
                var res = dbContext.Hairdressing_Ratings
                    .Where(r => r.UserId == idDto.Id)
                    .Select(r => r.Score)
                    .ToList();

                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                Console.WriteLine("HairdressingRating/GetAllUserScores milisegundos: " + elapsedMs);

                return res;
            }
        }

        /// <summary>
        ///Obtiene el puntaje promedio de una peluqueira dada.
        /// </summary>
        [HttpPost]
        public double GetUserRating([FromBody] IdDto idDto)
        {
            var watch = Stopwatch.StartNew();

            using (var dbContext = new ApplicationDbContext())
            {
                var ratings = dbContext.Hairdressing_Ratings.Where(r => r.UserId == idDto.Id).ToList();
                var res = ratings.Any() ? ratings.Average(r => r.Score) : 0;

                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                Console.WriteLine("HairdressingRating/GetUserRating milisegundos: " + elapsedMs);

                return res;
            }
        }
    }
}
