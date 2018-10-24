using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using SistemaTurnos.WebApplication.Database;
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
        /// dasdasdasdasdsa
        /// </summary>
        /// <param name="idDto"></param>
        /// <returns>OK</returns>
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
