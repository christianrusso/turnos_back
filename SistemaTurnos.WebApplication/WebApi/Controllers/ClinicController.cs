using GeoCoordinatePortable;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using SistemaTurnos.WebApplication.Database;
using SistemaTurnos.WebApplication.WebApi.Dto.Clinic;
using System.Collections.Generic;
using System.Linq;

namespace SistemaTurnos.WebApplication.WebApi.Controllers
{
    [Route("Api/[controller]/[action]")]
    [Produces("application/json")]
    [EnableCors("AnyOrigin")]
    public class ClinicController : Controller
    {
        [HttpPost]
        public List<ClinicDto> GetAllInRadius([FromBody] GeoLocationDto geoLocation)
        {
            var res = new List<ClinicDto>();
            var userLocation = new GeoCoordinate(geoLocation.Latitude, geoLocation.Longitude);

            using (var dbContext = new ApplicationDbContext())
            {
                var clinics = dbContext.Clinics.ToList();

                foreach (var clinic in clinics)
                {
                    var location = new GeoCoordinate(clinic.Latitude, clinic.Longitude);
                    var distanceToUser = location.GetDistanceTo(userLocation);

                    if (distanceToUser <= geoLocation.RadiusInMeters)
                    {
                        res.Add(new ClinicDto
                        {
                            ClinicId = clinic.Id,
                            Address = clinic.Address,
                            Latitude = clinic.Latitude,
                            Longitude = clinic.Longitude,
                            DistanceToUser = distanceToUser
                        });
                    }
                }
            }

            return res;
        }
    }
}
