using GeoCoordinatePortable;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using SistemaTurnos.WebApplication.Database;
using SistemaTurnos.WebApplication.Database.Model;
using SistemaTurnos.WebApplication.WebApi.Dto.Clinic;
using SistemaTurnos.WebApplication.WebApi.Dto.Rating;
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

        [HttpPost]
        public List<FullClinicDto> GetByFilter([FromBody] FilterClinicDto filterDto)
        {
            var res = new List<FullClinicDto>();
            var userLocation = filterDto.Location != null ? new GeoCoordinate(filterDto.Location.Latitude, filterDto.Location.Longitude) : null;

            using (var dbContext = new ApplicationDbContext())
            {
                // Filtro por ciudad
                var clinics = dbContext.Clinics
                    .Where(c => !filterDto.Cities.Any() || filterDto.Cities.Any(city => c.City == city))
                    .ToList();
                    
                var filteredClinics = new List<Clinic>();

                // Filtro por distancia
                if (userLocation != null)
                {
                    foreach (var clinic in clinics)
                    {
                        var location = new GeoCoordinate(clinic.Latitude, clinic.Longitude);
                        var distanceToUser = location.GetDistanceTo(userLocation);

                        if (distanceToUser <= filterDto.Location.RadiusInMeters)
                        {
                            filteredClinics.Add(clinic);
                        }
                    }
                }
                else
                {
                    filteredClinics = clinics;
                }

                foreach (var clinic in filteredClinics)
                {
                    var userId = clinic.UserId;

                    // Filtro por cantidad de puntuaciones
                    var ratings = dbContext.Ratings.Where(r => r.UserId == userId).ToList();

                    if (filterDto.ScoreQuantity.HasValue && ratings.Count < filterDto.ScoreQuantity)
                    {
                        continue;
                    }

                    // Filtro por puntuacion
                    var score = ratings.Any() ? ratings.Average(r => r.Score) : 0;

                    if (filterDto.Score.HasValue && score < filterDto.Score)
                    {
                        continue;
                    }

                    // Filtro por especialidades
                    var specialties = dbContext.Specialties.Where(s => s.UserId == userId).ToList();

                    foreach (var specialty in filterDto.Specialties)
                    {
                        if (!specialties.Any(s => s.Description == specialty))
                        {
                            continue;
                        }
                    }

                    // Filtro por subespecialidades
                    var subspecialties = dbContext.Subspecialties.Where(sp => sp.UserId == userId).ToList();

                    foreach (var subspecialty in filterDto.Subspecialties)
                    {
                        if (!subspecialties.Any(sp => sp.Description == subspecialty))
                        {
                            continue;
                        }
                    }

                    // Filtro por obras sociales
                    var medicalInsurances = dbContext.MedicalInsurances.Where(mi => mi.UserId == userId).ToList();

                    foreach (var medicalInsurance in filterDto.MedicalInsurances)
                    {
                        if (!medicalInsurances.Any(mi => mi.Description == medicalInsurance))
                        {
                            continue;
                        }
                    }

                    // La clinica paso todos los filtros y la agrego al resultado
                    res.Add(new FullClinicDto
                    {
                        ClinicId = clinic.Id,
                        Name = clinic.Name,
                        Description = clinic.Description,
                        City = clinic.City,
                        Address = clinic.Address,
                        Latitude = clinic.Latitude,
                        Longitude = clinic.Longitude,
                        DistanceToUser = userLocation != null ? new GeoCoordinate(clinic.Latitude, clinic.Longitude).GetDistanceTo(userLocation) : 0,
                        Score = score,
                        ScoreQuantity = ratings.Count,
                        Ratings = ratings.Select(r => new RatingDto { Score = r.Score, Comment = r.Comment }).ToList(),
                        Specialties = specialties.Select(s => s.Description).ToList(),
                        Subspecialties = subspecialties.Select(sp => sp.Description).ToList(),
                        MedicalInsurances = medicalInsurances.Select(mi => mi.Description).ToList()
                    });
                }
            }

            return res;
        }
    }
}
