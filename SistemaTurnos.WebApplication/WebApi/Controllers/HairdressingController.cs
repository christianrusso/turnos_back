using GeoCoordinatePortable;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaTurnos.Database;
using SistemaTurnos.WebApplication.WebApi.Dto;
using SistemaTurnos.WebApplication.WebApi.Dto.Common;
using SistemaTurnos.WebApplication.WebApi.Dto.Rating;
using System.Collections.Generic;
using System.Linq;
using SistemaTurnos.WebApplication.WebApi.Dto.Hairdressing;
using SistemaTurnos.WebApplication.WebApi.Dto.Subspecialty;
using SistemaTurnos.Database.HairdressingModel;
using SistemaTurnos.WebApplication.WebApi.Services;
using SistemaTurnos.Commons.Authorization;
using SistemaTurnos.Commons.Exceptions;
using System;
using Microsoft.AspNetCore.Identity;
using SistemaTurnos.Database.Model;
using SistemaTurnos.Database.ModelData;
using System.Diagnostics;

namespace SistemaTurnos.WebApplication.WebApi.Controllers
{
    [Route("Api/Hairdressing/[controller]/[action]")]
    [Produces("application/json")]
    [EnableCors("AnyOrigin")]
    public class HairdressingController : Controller
    {
        private BusinessPlaceService _service;
        private readonly UserManager<ApplicationUser> _userManager;

        public HairdressingController(UserManager<ApplicationUser> userManager)
        {
            _service = new BusinessPlaceService();
            _userManager = userManager;
        }

        [HttpPost]
        [Authorize(Roles = Roles.AdministratorAndEmployee)]
        public void UpdateOpenCloseHours([FromBody] HairdressingOpenCloseHoursDto hoursDto)
        {
            var watch = Stopwatch.StartNew();

            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _service.GetUserId(HttpContext);

                var HairdressingsToUpdate = dbContext.Hairdressings.FirstOrDefault(c => c.Id == hoursDto.HairdressingId && c.UserId == userId);

                if (HairdressingsToUpdate == null)
                {
                    throw new BadRequestException();
                }

                // Valido los datos de los horarios
                var openCloseHours = hoursDto.OpenCloseHours.OrderBy(wh => wh.DayNumber).ThenBy(wh => wh.Start).ToList();

                int previousDayNumber = -1;

                foreach (var och in openCloseHours)
                {
                    if (((int) och.DayNumber) <= previousDayNumber || och.Start > och.End)
                    {
                        throw new BadRequestException();
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

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("Hairdressing/UpdateOpenCloseHours milisegundos: " + elapsedMs);

        }

        [HttpPost]
        public List<HairdressingDto> GetAllInRadius([FromBody] HairdressingGeoLocationDto geoLocation)
        {
            var watch = Stopwatch.StartNew();

            var res = new List<HairdressingDto>();
            var userLocation = new GeoCoordinate(geoLocation.Latitude, geoLocation.Longitude);

            using (var dbContext = new ApplicationDbContext())
            {
                var hairdressings = dbContext.Hairdressings.Where(h => h.BusinessType == geoLocation.BusinessType).ToList();

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

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("Hairdressing/GetAllInRadius milisegundos: " + elapsedMs);

            return res;
        }

        /// <summary>
        /// Devuelve todas las peluquerias con los filtros dados.
        /// </summary>
        [HttpPost]
        public List<FullHairdressingDto> GetByFilter([FromBody] FilterHairdressingDto filterDto)
        {
            var watch = Stopwatch.StartNew();

            filterDto.Specialties = filterDto.Specialties ?? new List<int>();
            filterDto.Subspecialties = filterDto.Subspecialties ?? new List<int>();
            filterDto.Cities = filterDto.Cities ?? new List<int>();
            filterDto.Stars = filterDto.Stars ?? new List<int>();

            var res = new List<FullHairdressingDto>();
            var userLocation = filterDto.Location != null ? new GeoCoordinate(filterDto.Location.Latitude, filterDto.Location.Longitude) : null;

            using (var dbContext = new ApplicationDbContext())
            {
                var loggerUserId = _service.GetUserIdOrDefault(HttpContext);
                var favoriteHairdressings = loggerUserId.HasValue ? dbContext.Clients.First(c => c.UserId == loggerUserId).FavoriteHairdressing : new List<Hairdressing_ClientFavorite>();

                // Filtro por ciudad y por id
                var hairdressings = dbContext.Hairdressings
                    .Where(h => !filterDto.HairdressingId.HasValue || h.Id == filterDto.HairdressingId)
                    .Where(h => !filterDto.BusinessType.HasValue || h.BusinessType == filterDto.BusinessType.Value)
                    .Where(h => !filterDto.Cities.Any() || filterDto.Cities.Any(city => h.CityId == city))
                    .ToList();

                var filteredHairdressings = new List<Hairdressing>();

                // Filtro por distancia
                if (userLocation != null)
                {
                    foreach (var hairdressing in hairdressings)
                    {
                        var location = new GeoCoordinate(hairdressing.Latitude, hairdressing.Longitude);
                        var distanceToUser = location.GetDistanceTo(userLocation);

                        if (distanceToUser <= filterDto.Location.RadiusInMeters)
                        {
                            filteredHairdressings.Add(hairdressing);
                        }
                    }
                }
                else
                {
                    filteredHairdressings = hairdressings;
                }

                foreach (var hairdressing in filteredHairdressings)
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

                    // Filtro por estrellas
                    int stars = Convert.ToInt32(score / 2);

                    if (filterDto.Stars.Any() && !filterDto.Stars.Any(s => s == stars))
                    {
                        continue;
                    }

                    // Filtro por especialidades
                    var specialties = dbContext.Hairdressing_Specialties.Where(s => s.UserId == userId).ToList();

                    if (filterDto.Specialties.Any() && filterDto.Specialties.TrueForAll(sId => !specialties.Any(s => s.DataId == sId)))
                    {
                        continue;
                    }

                    // Filtro por subespecialidades
                    var subspecialties = dbContext.Hairdressing_Subspecialties.Where(sp => sp.UserId == userId).ToList();

                    if (filterDto.Subspecialties.Any() && filterDto.Subspecialties.TrueForAll(ssId => !subspecialties.Any(ss => ss.DataId == ssId)))
                    {
                        continue;
                    }

                    // Filtro por las que tienen algun turno disponible en el rango de dias especificado
                    if (filterDto.AvailableAppointmentStartDate.HasValue && filterDto.AvailableAppointmentEndDate.HasValue)
                    {
                        var professionals = dbContext.Hairdressing_Professionals
                            .Where(d => d.UserId == userId)
                            .Where(d => !filterDto.Specialties.Any() || filterDto.Specialties.Any(s => d.Subspecialties.Any(ssp => ssp.Subspecialty.Specialty.DataId == s)))
                            .Where(d => !filterDto.Subspecialties.Any() || filterDto.Subspecialties.Any(ss => d.Subspecialties.Any(ssp => ssp.Subspecialty.DataId == ss)))
                            .ToList();

                        var hasAppointmentAvailable = false;

                        foreach (var professional in professionals)
                        {
                            var professionalSubspecialties = professional.Subspecialties
                                .Where(ps => !filterDto.Specialties.Any() || filterDto.Specialties.Any(ssp => ssp == ps.Subspecialty.Specialty.DataId))
                                .Where(ps => !filterDto.Subspecialties.Any() || filterDto.Subspecialties.Any(ss => ss == ps.Subspecialty.DataId))
                                .ToList();

                            for (var day = filterDto.AvailableAppointmentStartDate.Value.Date; day <= filterDto.AvailableAppointmentEndDate.Value.Date; day = day.AddDays(1))
                            {
                                if (professionalSubspecialties.Any(ps => professional.GetAllAvailableAppointmentsForDay(day, ps.SubspecialtyId).Any()))
                                {
                                    hasAppointmentAvailable = true;
                                    break;
                                }
                            }

                            if (hasAppointmentAvailable)
                            {
                                break;
                            }
                        }
                        if (!hasAppointmentAvailable)
                        {
                            continue;
                        }
                    }

                    // La clinica paso todos los filtros y la agrego al resultado
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
                        Ratings = ratings.Select(r => new RatingDto { User = r.Appointment.Patient.Client.User.Email, Score = r.Score, Comment = r.Comment, DateTime = r.DateTime }).ToList(),
                        Specialties = specialties.Select(s => s.Data.Description).ToList(),
                        Subspecialties = subspecialties.Select(sp => new HairdressingSubspecialtyDto { Description = sp.Data.Description, Price = sp.Price }).ToList(),
                        Logo = hairdressing.Logo,
                        OpenCloseHours = hairdressing.OpenCloseHours.Select(och => new OpenCloseHoursDto { DayNumber = och.DayNumber, Start = och.Start, End = och.End }).ToList(),
                        IsFavorite = favoriteHairdressings.Any(f => f.HairdressingId == hairdressing.Id),
                        RequiresPayment = hairdressing.RequiresPayment,
                        Images = hairdressing.Images.Select(i => i.Data).ToList()
                    });
                }
            }

            // Ordenamiento
            if (filterDto.SortField == "score")
            {
                if (filterDto.AscendingOrder.HasValue && filterDto.AscendingOrder.Value)
                {
                    res = res.OrderBy(h => h.Score).ToList();
                }
                else
                {
                    res = res.OrderByDescending(h => h.Score).ToList();
                }
            }
            else if (filterDto.SortField == "comments")
            {
                if (filterDto.AscendingOrder.HasValue && filterDto.AscendingOrder.Value)
                {
                    res = res.OrderBy(h => h.Ratings.Count).ToList();
                }
                else
                {
                    res = res.OrderByDescending(h => h.Ratings.Count).ToList();
                }
            }
            else
            {
                res = res.OrderByDescending(h => h.Score).ToList();
            }

            // Paginacion
            int resultSize = res.Count;
            int from = filterDto.From ?? 0;
            int to = filterDto.To - from ?? res.Count;
            res = res.Skip(from).Take(to).ToList();

            foreach (var hairdressing in res)
            {
                hairdressing.ResultSize = resultSize;
            }

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("Hairdressing/GetByFilter milisegundos: " + elapsedMs);

            return res;
        }

        [HttpPost]
        [Authorize(Roles = Roles.Client)]
        public bool IsPatientOfHairdressing([FromBody] IdDto idDto)
        {
            var watch = Stopwatch.StartNew();

            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _service.GetUserId(HttpContext);

                var hairdressing = dbContext.Hairdressings.FirstOrDefault(c => c.Id == idDto.Id);

                if (hairdressing == null)
                {
                    throw new BadRequestException();
                }

                var client = dbContext.Clients.FirstOrDefault(c => c.UserId == userId);

                var patient = client.HairdressingPatients.FirstOrDefault(p => p.UserId == hairdressing.UserId);

                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                Console.WriteLine("Hairdressing/IsPatientOfHairdressing milisegundos: " + elapsedMs);

                return patient != null;
            }
        }

        [HttpPost]
        [Authorize(Roles = Roles.AdministratorAndEmployee)]
        public void ChangePayment([FromBody] RequireDto dto)
        {
            var watch = Stopwatch.StartNew();

            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _service.GetUserId(HttpContext);

                var hairdressingToUpdate = dbContext.Hairdressings.FirstOrDefault(h => h.UserId == userId);

                if (hairdressingToUpdate == null)
                {
                    throw new BadRequestException();
                }

                hairdressingToUpdate.RequiresPayment = dto.Require;
                hairdressingToUpdate.ClientId = dto.Require ? dto.ClientId : string.Empty;
                hairdressingToUpdate.ClientSecret = dto.Require ? dto.ClientSecret : string.Empty;

                dbContext.SaveChanges();
            }

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("Hairdressing/ChangePayment milisegundos: " + elapsedMs);
        }

        /// <summary>
        /// Actualiza los datos de una peluqueria dada.
        /// </summary>
        [HttpPost]
        [Authorize(Roles = Roles.Administrator)]
        public void UpdateInformation([FromBody] UpdateHairdressingDto hairdressingDto)
        {
            var watch = Stopwatch.StartNew();

            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _service.GetUserId(HttpContext);

                var hairdressingToUpdate = dbContext.Hairdressings.FirstOrDefault(c => c.UserId == userId);

                if (hairdressingToUpdate == null)
                {
                    throw new BadRequestException();
                }

                if (!string.IsNullOrWhiteSpace(hairdressingDto.Name))
                {
                    hairdressingToUpdate.Name = hairdressingDto.Name;
                }

                if (!string.IsNullOrWhiteSpace(hairdressingDto.Address))
                {
                    hairdressingToUpdate.Address = hairdressingDto.Address;
                }

                if (!string.IsNullOrWhiteSpace(hairdressingDto.Description))
                {
                    hairdressingToUpdate.Description = hairdressingDto.Description;
                }

                if (hairdressingDto.CityId.HasValue)
                {
                    hairdressingToUpdate.CityId = hairdressingDto.CityId.Value;
                }

                if (hairdressingDto.Latitude.HasValue)
                {
                    hairdressingToUpdate.Latitude = hairdressingDto.Latitude.Value;
                }

                if (hairdressingDto.Longitude.HasValue)
                {
                    hairdressingToUpdate.Longitude = hairdressingDto.Longitude.Value;
                }

                if (!string.IsNullOrWhiteSpace(hairdressingDto.Logo))
                {
                    hairdressingToUpdate.Logo = hairdressingDto.Logo;
                }

                if (hairdressingDto.OpenCloseHours.Any())
                {
                    // Valido los datos de los horarios
                    var openCloseHours = hairdressingDto.OpenCloseHours.OrderBy(wh => wh.DayNumber).ThenBy(wh => wh.Start).ToList();

                    int previousDayNumber = -1;

                    foreach (var och in openCloseHours)
                    {
                        if (((int)och.DayNumber) <= previousDayNumber || och.Start > och.End)
                        {
                            throw new BadRequestException();
                        }

                        previousDayNumber = (int)och.DayNumber;
                    }

                    hairdressingToUpdate.OpenCloseHours.ForEach(och => dbContext.Entry(och).State = EntityState.Deleted);

                    var newOpenCloseHours = hairdressingDto.OpenCloseHours.Select(wh => new Hairdressing_OpenCloseHours
                    {
                        DayNumber = wh.DayNumber,
                        Start = wh.Start,
                        End = wh.End
                    }).ToList();

                    hairdressingToUpdate.OpenCloseHours = newOpenCloseHours;
                }

                if (!string.IsNullOrWhiteSpace(hairdressingDto.OldPassword) && !string.IsNullOrWhiteSpace(hairdressingDto.NewPassword))
                {
                    var appUser = _userManager.Users.SingleOrDefault(user => user.Id == userId);

                    if (appUser == null)
                    {
                        throw new ApplicationException(ExceptionMessages.UserDoesNotExists);
                    }

                    var resultChangePassword = _userManager.ChangePasswordAsync(appUser, hairdressingDto.OldPassword, hairdressingDto.NewPassword).Result;

                    if (!resultChangePassword.Succeeded)
                    {
                        throw new ApplicationException(ExceptionMessages.InternalServerError);
                    }
                }

                if (hairdressingDto.Images.Any())
                {
                    hairdressingToUpdate.Images.ForEach(i => dbContext.Entry(i).State = EntityState.Deleted);
                    hairdressingToUpdate.Images = hairdressingDto.Images.Select(i => new Image
                    {
                        Data = i,
                        UserId = userId
                    }).ToList();
                }

                hairdressingToUpdate.RequiresPayment = hairdressingDto.Require;
                hairdressingToUpdate.ClientId = hairdressingDto.Require ? hairdressingDto.ClientId : string.Empty;
                hairdressingToUpdate.ClientSecret = hairdressingDto.Require ? hairdressingDto.ClientSecret : string.Empty;

                dbContext.SaveChanges();
            }

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("Hairdressing/UpdateInformation milisegundos: " + elapsedMs);
        }
    }
}
