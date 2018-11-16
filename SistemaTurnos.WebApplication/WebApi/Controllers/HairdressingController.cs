using GeoCoordinatePortable;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaTurnos.Database;
using SistemaTurnos.WebApplication.WebApi.Authorization;
using SistemaTurnos.WebApplication.WebApi.Dto;
using SistemaTurnos.WebApplication.WebApi.Dto.Common;
using SistemaTurnos.WebApplication.WebApi.Dto.Rating;
using SistemaTurnos.WebApplication.WebApi.Exceptions;
using System.Collections.Generic;
using System.Linq;
using SistemaTurnos.WebApplication.WebApi.Dto.Hairdressing;
using SistemaTurnos.Database.HairdressingModel;
using SistemaTurnos.WebApplication.WebApi.Services;

namespace SistemaTurnos.WebApplication.WebApi.Controllers
{
    [Route("Api/Hairdressing/[controller]/[action]")]
    [Produces("application/json")]
    [EnableCors("AnyOrigin")]
    public class HairdressingController : Controller
    {
        private BusinessPlaceService _service;

        public HairdressingController()
        {
            _service = new BusinessPlaceService(HttpContext);
        }

        [HttpPost]
        [Authorize(Roles = Roles.AdministratorAndEmployee)]
        public void UpdateOpenCloseHours([FromBody] HairdressingOpenCloseHoursDto hoursDto)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _service.GetUserId(this.HttpContext);

                var HairdressingsToUpdate = dbContext.Hairdressings.FirstOrDefault(c => c.Id == hoursDto.HairdressingId && c.UserId == userId);

                if (HairdressingsToUpdate == null)
                {
                    throw new BadRequestException(ExceptionMessages.BadRequest);
                }

                // Valido los datos de los horarios
                var openCloseHours = hoursDto.OpenCloseHours.OrderBy(wh => wh.DayNumber).ThenBy(wh => wh.Start).ToList();

                int previousDayNumber = -1;

                foreach (var och in openCloseHours)
                {
                    if (((int) och.DayNumber) <= previousDayNumber || och.Start > och.End)
                    {
                        throw new BadRequestException(ExceptionMessages.BadRequest);
                    }

                    previousDayNumber = (int)och.DayNumber;
                }

                HairdressingsToUpdate.OpenCloseHours.ForEach(och => dbContext.Entry(och).State = EntityState.Deleted);

                var newOpenCloseHours = hoursDto.OpenCloseHours.Select(wh => new Hairdressing_OpenCloseHours
                {
                    DayNumber = wh.DayNumber,
                    Start = wh.Start,
                    End = wh.End
                }).ToList();

                HairdressingsToUpdate.OpenCloseHours = newOpenCloseHours;

                dbContext.SaveChanges();
            }
        }

        [HttpPost]
        public List<HairdressingDto> GetAllInRadius([FromBody] GeoLocationDto geoLocation)
        {
            var res = new List<HairdressingDto>();
            var userLocation = new GeoCoordinate(geoLocation.Latitude, geoLocation.Longitude);

            using (var dbContext = new ApplicationDbContext())
            {
                var hairdressings = dbContext.Hairdressings.ToList();

                foreach (var hairdressing in hairdressings)
                {
                    var location = new GeoCoordinate(hairdressing.Latitude, hairdressing.Longitude);
                    var distanceToUser = location.GetDistanceTo(userLocation);

                    if (distanceToUser <= geoLocation.RadiusInMeters)
                    {
                        res.Add(new HairdressingDto
                        {
                            HairdressingId = hairdressing.Id,
                            Address = hairdressing.Address,
                            Latitude = hairdressing.Latitude,
                            Longitude = hairdressing.Longitude,
                            DistanceToUser = distanceToUser
                        });
                    }
                }
            }

            return res;
        }

        [HttpPost]
        public List<FullHairdressingDto> GetByFilter([FromBody] FilterHairdressingDto filterDto)
        {
            var res = new List<FullHairdressingDto>();
            var userLocation = filterDto.Location != null ? new GeoCoordinate(filterDto.Location.Latitude, filterDto.Location.Longitude) : null;

            using (var dbContext = new ApplicationDbContext())
            {
                // Filtro por ciudad y por id
                var hairdressings = dbContext.Hairdressings
                    .Where(c => !filterDto.HairdressingId.HasValue || c.Id == filterDto.HairdressingId)
                    .Where(c => !filterDto.Cities.Any() || filterDto.Cities.Any(city => c.CityId == city))
                    .ToList();
                    
                var filteredHairdressing = new List<Hairdressing>();

                // Filtro por distancia
                if (userLocation != null)
                {
                    foreach (var hairdressing in hairdressings)
                    {
                        var location = new GeoCoordinate(hairdressing.Latitude, hairdressing.Longitude);
                        var distanceToUser = location.GetDistanceTo(userLocation);

                        if (distanceToUser <= filterDto.Location.RadiusInMeters)
                        {
                            filteredHairdressing.Add(hairdressing);
                        }
                    }
                }
                else
                {
                    filteredHairdressing = hairdressings;
                }

                foreach (var hairdressing in filteredHairdressing)
                {
                    var userId = hairdressing.UserId;

                    // Filtro por cantidad de puntuaciones
                    var ratings = dbContext.Hairdressing_Ratings.Where(r => r.UserId == userId).ToList();

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

                    var filtered = false;

                    // Filtro por especialidades
                    var specialties = dbContext.Hairdressing_Specialties.Where(s => s.UserId == userId).ToList();
                    
                    foreach (var specialtyId in filterDto.Specialties)
                    {
                        if (!specialties.Any(s => s.DataId == specialtyId))
                        {
                            filtered = true;
                            break;
                        }
                    }

                    if (filtered) continue;

                    // Filtro por subespecialidades
                    var subspecialties = dbContext.Hairdressing_Subspecialties.Where(sp => sp.UserId == userId).ToList();

                    foreach (var subspecialtyId in filterDto.Subspecialties)
                    {
                        if (!subspecialties.Any(sp => sp.DataId == subspecialtyId))
                        {
                            filtered = true;
                            break;
                        }
                    }

                    if (filtered) continue;

                    // Filtro por las que tienen algun turno disponible en el dia especificado
                    if (filterDto.AvailableAppointmentDate.HasValue)
                    {
                        var doctors = dbContext.Hairdressing_Professionals
                            .Where(d => d.UserId == userId)
                            .Where(d => !filterDto.Specialties.Any() || filterDto.Specialties.Any(s => s == d.Specialty.DataId))
                            .Where(d => !filterDto.Subspecialties.Any() || filterDto.Subspecialties.Any(ss => ss == d.Subspecialty.DataId))
                            .ToList();

                        var hasAppointmentAvailable = false;

                        foreach (var doctor in doctors)
                        {
                            if (doctor.GetAllAvailablesForDay(filterDto.AvailableAppointmentDate.Value).Any())
                            {
                                hasAppointmentAvailable = true;
                                break;
                            }
                        }

                        if (!hasAppointmentAvailable)
                        {
                            filtered = true;
                        }

                        if (filtered) continue;
                    }


                    // La Hairdressing paso todos los filtros y la agrego al resultado
                    res.Add(new FullHairdressingDto
                    {
                        HairdressingId = hairdressing.Id,
                        Name = hairdressing.Name,
                        Description = hairdressing.Description,
                        City = hairdressing.City.Name,
                        Address = hairdressing.Address,
                        Latitude = hairdressing.Latitude,
                        Longitude = hairdressing.Longitude,
                        DistanceToUser = userLocation != null ? new GeoCoordinate(hairdressing.Latitude, hairdressing.Longitude).GetDistanceTo(userLocation) : 0,
                        Score = score,
                        ScoreQuantity = ratings.Count,
                        Ratings = ratings.Select(r => new RatingDto { User = r.User.Email, Score = r.Score, Comment = r.Comment, DateTime = r.DateTime }).ToList(),
                        Specialties = specialties.Select(s => s.Data.Description).ToList(),
                        Subspecialties = subspecialties.Select(sp => sp.Data.Description).ToList(),
                        Logo = hairdressing.Logo,
                        OpenCloseHours = hairdressing.OpenCloseHours.Select(och => new OpenCloseHoursDto { DayNumber = och.DayNumber, Start = och.Start, End = och.End }).ToList()
                    });
                }
            }

            return res.OrderByDescending(c => c.Score).ToList();
        }

        [HttpPost]
        [Authorize(Roles = Roles.Client)]
        public bool IsPatientOfHairdressing([FromBody] IdDto idDto)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _service.GetUserId(this.HttpContext);

                var hairdressing = dbContext.Hairdressings.FirstOrDefault(c => c.Id == idDto.Id);

                if (hairdressing == null)
                {
                    throw new BadRequestException(ExceptionMessages.BadRequest);
                }

                var client = dbContext.Clients.FirstOrDefault(c => c.UserId == userId);

                var patient = client.HairdressingPatients.FirstOrDefault(p => p.UserId == hairdressing.UserId);

                return patient != null;
            }
        }
    }
}
