using GeoCoordinatePortable;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaTurnos.Database;
using SistemaTurnos.Database.ClinicModel;
using SistemaTurnos.WebApplication.WebApi.Dto;
using SistemaTurnos.WebApplication.WebApi.Dto.Common;
using SistemaTurnos.WebApplication.WebApi.Dto.Clinic;
using SistemaTurnos.WebApplication.WebApi.Dto.Rating;
using System.Collections.Generic;
using System.Linq;
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
    [Route("Api/[controller]/[action]")]
    [Produces("application/json")]
    [EnableCors("AnyOrigin")]
    public class ClinicController : Controller
    {
        private BusinessPlaceService _service;
        private readonly UserManager<ApplicationUser> _userManager;

        public ClinicController(UserManager<ApplicationUser> userManager)
        {
            _service = new BusinessPlaceService();
            _userManager = userManager;
        }

        /// <summary>
        /// Actualiza los horarios de abrir y cerrar de una clinica.
        /// </summary>
        [HttpPost]
        [Authorize(Roles = Roles.AdministratorAndEmployee)]
        public void UpdateOpenCloseHours([FromBody] ClinicOpenCloseHoursDto hoursDto)
        {
            var watch = Stopwatch.StartNew();

            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _service.GetUserId(HttpContext);

                Clinic clinicToUpdate = dbContext.Clinics.FirstOrDefault(c => c.Id == hoursDto.ClinicId && c.UserId == userId);

                if (clinicToUpdate == null)
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

                clinicToUpdate.OpenCloseHours.ForEach(och => dbContext.Entry(och).State = EntityState.Deleted);

                var newOpenCloseHours = hoursDto.OpenCloseHours.Select(wh => new Clinic_OpenCloseHours
                {
                    DayNumber = wh.DayNumber,
                    Start = wh.Start,
                    End = wh.End
                }).ToList();

                clinicToUpdate.OpenCloseHours = newOpenCloseHours;

                dbContext.SaveChanges();

                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                Console.WriteLine("ClinicController/UpdateOpenCloseHours milisegundos: " + elapsedMs);
            }
        }

        /// <summary>
        /// Actualiza los datos de una clinica dada.
        /// </summary>
        [HttpPost]
        [Authorize(Roles = Roles.Administrator)]
        public void UpdateInformation([FromBody] UpdateClinicDto clinicDto)
        {
            var watch = Stopwatch.StartNew();

            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _service.GetUserId(HttpContext);

                var clinicToUpdate = dbContext.Clinics.FirstOrDefault(c => c.UserId == userId);

                if (clinicToUpdate == null)
                {
                    throw new BadRequestException();
                }

                if (!string.IsNullOrWhiteSpace(clinicDto.Name))
                {
                    clinicToUpdate.Name = clinicDto.Name;
                }

                if (!string.IsNullOrWhiteSpace(clinicDto.Address))
                {
                    clinicToUpdate.Address = clinicDto.Address;
                }

                if (!string.IsNullOrWhiteSpace(clinicDto.Description))
                {
                    clinicToUpdate.Description = clinicDto.Description;
                }

                if (clinicDto.CityId.HasValue)
                {
                    clinicToUpdate.CityId = clinicDto.CityId.Value;
                }

                if (clinicDto.Latitude.HasValue)
                {
                    clinicToUpdate.Latitude = clinicDto.Latitude.Value;
                }

                if (clinicDto.Longitude.HasValue)
                {
                    clinicToUpdate.Longitude = clinicDto.Longitude.Value;
                }

                if (!string.IsNullOrWhiteSpace(clinicDto.Logo))
                {
                    clinicToUpdate.Logo = clinicDto.Logo;
                }

                if (clinicDto.OpenCloseHours.Any())
                {
                    // Valido los datos de los horarios
                    var openCloseHours = clinicDto.OpenCloseHours.OrderBy(wh => wh.DayNumber).ThenBy(wh => wh.Start).ToList();

                    int previousDayNumber = -1;

                    foreach (var och in openCloseHours)
                    {
                        if (((int)och.DayNumber) <= previousDayNumber || och.Start > och.End)
                        {
                            throw new BadRequestException();
                        }

                        previousDayNumber = (int)och.DayNumber;
                    }

                    clinicToUpdate.OpenCloseHours.ForEach(och => dbContext.Entry(och).State = EntityState.Deleted);

                    var newOpenCloseHours = clinicDto.OpenCloseHours.Select(wh => new Clinic_OpenCloseHours
                    {
                        DayNumber = wh.DayNumber,
                        Start = wh.Start,
                        End = wh.End
                    }).ToList();

                    clinicToUpdate.OpenCloseHours = newOpenCloseHours;
                }

                if (!string.IsNullOrWhiteSpace(clinicDto.OldPassword) && !string.IsNullOrWhiteSpace(clinicDto.NewPassword))
                {
                    var appUser = _userManager.Users.SingleOrDefault(user => user.Id == userId);

                    if (appUser == null)
                    {
                        throw new ApplicationException(ExceptionMessages.UserDoesNotExists);
                    }

                    var resultChangePassword = _userManager.ChangePasswordAsync(appUser, clinicDto.OldPassword, clinicDto.NewPassword).Result;

                    if (!resultChangePassword.Succeeded)
                    {
                        throw new ApplicationException(ExceptionMessages.InternalServerError);
                    }
                }

                if (clinicDto.Images.Any())
                {
                    clinicToUpdate.Images.ForEach(i => dbContext.Entry(i).State = EntityState.Deleted);
                    clinicToUpdate.Images = clinicDto.Images.Select(i => new Image
                    {
                        Data = i,
                        UserId = userId
                    }).ToList();
                }

                dbContext.SaveChanges();

                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                Console.WriteLine("ClinicController/UpdateInformation milisegundos: " + elapsedMs);
            }
        }

        /// <summary>
        /// Devuelve todas las clinicas en un radio dado.
        /// </summary>
        [HttpPost]
        public List<ClinicDto> GetAllInRadius([FromBody] GeoLocationDto geoLocation)
        {
            var watch = Stopwatch.StartNew();

            var res = new List<ClinicDto>();
            var userLocation = new GeoCoordinate(geoLocation.Latitude, geoLocation.Longitude);

            using (var dbContext = new ApplicationDbContext())
            {
                var clinics = dbContext.Clinics.ToList();

                foreach (var clinic in clinics)
                {
                    var location = new GeoCoordinate(clinic.Latitude, clinic.Longitude);
                    var distanceToUser = location.GetDistanceTo(userLocation);
                    var clinicUserId = clinic.UserId;
                    var ratings = dbContext.Clinic_Ratings.Where(r => r.UserId == clinicUserId).ToList();
                    var score = ratings.Any() ? ratings.Average(r => r.Score) : 0;

                    if (distanceToUser <= geoLocation.RadiusInMeters)
                    {
                        res.Add(new ClinicDto
                        {
                            ClinicId = clinic.Id,
                            Address = clinic.Address,
                            Latitude = clinic.Latitude,
                            Longitude = clinic.Longitude,
                            DistanceToUser = distanceToUser,
                            City = clinic.City.Name,
                            Name = clinic.Name,
                            Description = clinic.Description,
                            Logo = clinic.Logo,
                            Score = score
                        });
                    }
                }
            }

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("ClinicController/GetAllInRadius milisegundos: " + elapsedMs);

            return res;
        }

        /// <summary>
        /// Devuelve todas las clinicas con los filtros dados.
        /// </summary>
        [HttpPost]
        public List<FullClinicDto> GetByFilter([FromBody] FilterClinicDto filterDto)
        {
            var watch = Stopwatch.StartNew();

            filterDto.Specialties = filterDto.Specialties ?? new List<int>();
            filterDto.Subspecialties = filterDto.Subspecialties ?? new List<int>();
            filterDto.MedicalInsurances = filterDto.MedicalInsurances ?? new List<int>();
            filterDto.MedicalPlans = filterDto.MedicalPlans ?? new List<int>();
            filterDto.Cities = filterDto.Cities ?? new List<int>();
            filterDto.Stars = filterDto.Stars ?? new List<int>();

            var res = new List<FullClinicDto>();
            var userLocation = filterDto.Location != null ? new GeoCoordinate(filterDto.Location.Latitude, filterDto.Location.Longitude) : null;

            using (var dbContext = new ApplicationDbContext())
            {
                var loggerUserId = _service.GetUserIdOrDefault(HttpContext);
                var favoriteClinics = loggerUserId.HasValue ? dbContext.Clients.First(c => c.UserId == loggerUserId).FavoriteClinics : new List<Clinic_ClientFavorite>();
                
                // Filtro por ciudad y por id
                var clinics = dbContext.Clinics
                    .Where(c => !filterDto.ClinicId.HasValue || c.Id == filterDto.ClinicId)
                    .Where(c => !filterDto.Cities.Any() || filterDto.Cities.Any(city => c.CityId == city))
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
                    var ratings = dbContext.Clinic_Ratings.Where(r => r.UserId == userId).ToList();

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

                    if (filterDto.Stars.Any () && !filterDto.Stars.Any(s => s == stars))
                    {
                        continue;
                    }

                    // Filtro por especialidades
                    var specialties = dbContext.Clinic_Specialties.Where(s => s.UserId == userId).ToList();

                    if (filterDto.Specialties.Any() && filterDto.Specialties.TrueForAll(sId => !specialties.Any(s => s.DataId == sId)))
                    {
                        continue;
                    }

                    // Filtro por subespecialidades
                    var subspecialties = dbContext.Clinic_Subspecialties.Where(sp => sp.UserId == userId).ToList();

                    if (filterDto.Subspecialties.Any() && filterDto.Subspecialties.TrueForAll(ssId => !subspecialties.Any(ss => ss.DataId == ssId)))
                    {
                        continue;
                    }

                    // Filtro por obras sociales
                    var medicalInsurances = dbContext.Clinic_MedicalInsurances.Where(mi => mi.UserId == userId).ToList();

                    if (filterDto.MedicalInsurances.Any() && (filterDto.MedicalInsurances.TrueForAll(miId => !medicalInsurances.Any(mi => mi.DataId == miId))))
                    {
                        continue;
                    }

                    // Filtro por planes de obras sociales
                    var medicalPlans = dbContext.Clinic_MedicalPlans.Where(mp => mp.UserId == userId).ToList();

                    if (filterDto.MedicalPlans.Any() && filterDto.MedicalPlans.TrueForAll(mpId => !medicalPlans.Any(mp => mp.DataId == mpId)))
                    {
                        continue;
                    }

                    // Filtro por las que tienen algun turno disponible en el rango de dias especificado
                    if (filterDto.AvailableAppointmentStartDate.HasValue && filterDto.AvailableAppointmentEndDate.HasValue)
                    {
                        var doctors = dbContext.Clinic_Doctors
                            .Where(d => d.UserId == userId)
                            .Where(d => !filterDto.Specialties.Any() || filterDto.Specialties.Any(s => d.Subspecialties.Any(ssp => ssp.Subspecialty.Specialty.DataId == s)))
                            .Where(d => !filterDto.Subspecialties.Any() || filterDto.Subspecialties.Any(ss => d.Subspecialties.Any(ssp => ssp.Subspecialty.DataId == ss)))
                            .ToList();

                        var hasAppointmentAvailable = false;

                        foreach (var doctor in doctors)
                        {
                            var doctorSubspecialties = doctor.Subspecialties
                                .Where(ds => !filterDto.Specialties.Any() || filterDto.Specialties.Any(ssp => ssp == ds.Subspecialty.Specialty.DataId))
                                .Where(ds => !filterDto.Subspecialties.Any() || filterDto.Subspecialties.Any(ss => ss == ds.Subspecialty.DataId))
                                .ToList();

                            for (var day = filterDto.AvailableAppointmentStartDate.Value.Date; day <= filterDto.AvailableAppointmentEndDate.Value.Date; day = day.AddDays(1))
                            {
                                if (doctorSubspecialties.Any(ds => doctor.GetAllAvailableAppointmentsForDay(day, ds.SubspecialtyId).Any()))
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
                    res.Add(new FullClinicDto
                    {
                        ClinicId = clinic.Id,
                        Name = clinic.Name,
                        Description = clinic.Description,
                        City = clinic.City.Name,
                        Address = clinic.Address,
                        Latitude = clinic.Latitude,
                        Longitude = clinic.Longitude,
                        DistanceToUser = userLocation != null ? new GeoCoordinate(clinic.Latitude, clinic.Longitude).GetDistanceTo(userLocation) : 0,
                        Score = score,
                        ScoreQuantity = ratings.Count,
                        Ratings = ratings.Select(r => new RatingDto { User = r.Appointment.Patient.Client.User.Email, Score = r.Score, Comment = r.Comment, DateTime = r.DateTime }).ToList(),
                        Specialties = specialties.Select(s => s.Data.Description).ToList(),
                        Subspecialties = subspecialties.Select(sp => sp.Data.Description).ToList(),
                        MedicalInsurances = medicalInsurances.Select(mi => mi.Data.Description).ToList(),
                        MedicalPlans = medicalPlans.Select(mp => mp.Data.Description).ToList(),
                        Logo = clinic.Logo,
                        OpenCloseHours = clinic.OpenCloseHours.Select(och => new OpenCloseHoursDto { DayNumber = och.DayNumber, Start = och.Start, End = och.End }).ToList(),
                        IsFavorite = favoriteClinics.Any(f => f.ClinicId == clinic.Id),
                        RequiresPayment = false,
                        Images = clinic.Images.Select(i => i.Data).ToList()
                    });
                }
            }

            // Ordenamiento
            if (filterDto.SortField == "score")
            {
                if (filterDto.AscendingOrder.HasValue && filterDto.AscendingOrder.Value)
                {
                    res = res.OrderBy(c => c.Score).ToList();
                } else
                {
                    res = res.OrderByDescending(c => c.Score).ToList();
                }
            }
            else if (filterDto.SortField == "comments")
            {
                if (filterDto.AscendingOrder.HasValue && filterDto.AscendingOrder.Value)
                {
                    res = res.OrderBy(c => c.Ratings.Count).ToList();
                }
                else
                {
                    res = res.OrderByDescending(c => c.Ratings.Count).ToList();
                }
            }
            else
            {
                res = res.OrderByDescending(c => c.Score).ToList();
            }
            
            // Paginacion
            int resultSize = res.Count;
            int from = filterDto.From ?? 0;
            int to = filterDto.To - from ?? res.Count;
            res = res.Skip(from).Take(to).ToList();

            foreach (var clinic in res)
            {
                clinic.ResultSize = resultSize;
            }

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("ClinicController/GetByFilter milisegundos: " + elapsedMs);

            return res;
        }

        /// <summary>
        /// Devuelve si un usuario es paciente de una clinica dada.
        /// </summary>
        [HttpPost]
        [Authorize(Roles = Roles.Client)]
        public bool IsPatientOfClinic([FromBody] IdDto idDto)
        {
            var watch = Stopwatch.StartNew();

            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _service.GetUserId(HttpContext);

                var clinic = dbContext.Clinics.FirstOrDefault(c => c.Id == idDto.Id);

                if (clinic == null)
                {
                    throw new BadRequestException();
                }

                var client = dbContext.Clients.FirstOrDefault(c => c.UserId == userId);

                var patient = client.Patients.FirstOrDefault(p => p.UserId == clinic.UserId);

                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                Console.WriteLine("ClinicController/IsPatientOfClinic milisegundos: " + elapsedMs);

                return patient != null;
            }
        }
    }
}
