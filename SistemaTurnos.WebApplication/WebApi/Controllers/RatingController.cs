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
            var watch = Stopwatch.StartNew();

            using (var dbContext = new ApplicationDbContext())
            {
                var res = dbContext.Clinic_Ratings
                    .Where(r => r.Appointment.Patient.Client.UserId == idDto.Id)
                    .Select(r => r.Comment)
                    .ToList();

                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                Console.WriteLine("RatingController/GetAllUserComments milisegundos: " + elapsedMs);

                return res;
            }
        }

        /// <summary>
        ///Devuelve todos los puntajes de una clinica dada.
        /// </summary>
        [HttpPost]
        public List<uint> GetAllUserScores([FromBody] IdDto idDto)
        {
            var watch = Stopwatch.StartNew();

            using (var dbContext = new ApplicationDbContext())
            {
                var res = dbContext.Clinic_Ratings
                    .Where(r => r.Appointment.Patient.Client.UserId == idDto.Id)
                    .Select(r => r.Score)
                    .ToList();

                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                Console.WriteLine("RatingController/GetAllUserScores milisegundos: " + elapsedMs);

                return res;
            }
        }

        /// <summary>
        ///Devuelve el promedio del puntaje de una clinica dada.
        /// </summary>
        [HttpPost]
        public double GetUserRating([FromBody] IdDto idDto)
        {
            var watch = Stopwatch.StartNew();

            using (var dbContext = new ApplicationDbContext())
            {
                var ratings = dbContext.Clinic_Ratings.Where(r => r.Appointment.Patient.Client.UserId == idDto.Id).ToList();
                var res = ratings.Any() ? ratings.Average(r => r.Score) : 0;

                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                Console.WriteLine("RatingController/GetUserRating milisegundos: " + elapsedMs);

                return res;
            }
        }
    }
}
