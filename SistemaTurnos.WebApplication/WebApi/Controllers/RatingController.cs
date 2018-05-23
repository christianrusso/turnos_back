using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using SistemaTurnos.WebApplication.Database;
using SistemaTurnos.WebApplication.WebApi.Dto;
using SistemaTurnos.WebApplication.WebApi.Exceptions;
using System;
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
                return dbContext.Ratings
                    .Where(r => r.UserId == idDto.Id)
                    .Select(r => r.Comment)
                    .ToList();
            }
        }

        [HttpPost]
        public List<uint> GetAllUserScores([FromBody] IdDto idDto)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                return dbContext.Ratings
                    .Where(r => r.UserId == idDto.Id)
                    .Select(r => r.Score)
                    .ToList();
            }
        }

        [HttpPost]
        public double GetUserRating([FromBody] IdDto idDto)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var ratings = dbContext.Ratings.Where(r => r.UserId == idDto.Id).ToList();
                return ratings.Any() ? ratings.Average(r => r.Score) : 0;
            }
        }
    }
}
