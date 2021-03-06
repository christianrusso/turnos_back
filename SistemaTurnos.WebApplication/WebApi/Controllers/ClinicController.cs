﻿using GeoCoordinatePortable;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaTurnos.WebApplication.Database;
using SistemaTurnos.WebApplication.Database.ClinicModel;
using SistemaTurnos.WebApplication.WebApi.Authorization;
using SistemaTurnos.WebApplication.WebApi.Dto;
using SistemaTurnos.WebApplication.WebApi.Dto.Clinic;
using SistemaTurnos.WebApplication.WebApi.Dto.Rating;
using SistemaTurnos.WebApplication.WebApi.Exceptions;
using System;
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
        [Authorize(Roles = Roles.AdministratorAndEmployee)]
        public void UpdateOpenCloseHours([FromBody] ClinicOpenCloseHoursDto hoursDto)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

                Clinic clinicToUpdate = dbContext.Clinics.FirstOrDefault(c => c.Id == hoursDto.ClinicId && c.UserId == userId);

                if (clinicToUpdate == null)
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

                clinicToUpdate.OpenCloseHours.ForEach(och => dbContext.Entry(och).State = EntityState.Deleted);

                var newOpenCloseHours = hoursDto.OpenCloseHours.Select(wh => new Clinic_OpenCloseHours
                {
                    DayNumber = wh.DayNumber,
                    Start = wh.Start,
                    End = wh.End
                }).ToList();

                clinicToUpdate.OpenCloseHours = newOpenCloseHours;

                dbContext.SaveChanges();
            }
        }

        [HttpPost]
        [Authorize(Roles = Roles.Administrator)]
        public void UpdateInformation([FromBody] UpdateClinicDto clinicDto)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

                var clinicToUpdate = dbContext.Clinics.FirstOrDefault(c => c.UserId == userId);

                if (clinicToUpdate == null)
                {
                    throw new BadRequestException(ExceptionMessages.BadRequest);
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

                dbContext.SaveChanges();
            }
        }

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
                            DistanceToUser = distanceToUser,
                            City = clinic.City.Name,
                            Name = clinic.Name,
                            Description = clinic.Description,
                            Logo = clinic.Logo
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
                            .Where(d => !filterDto.Specialties.Any() || filterDto.Specialties.Any(s => s == d.Specialty.DataId))
                            .Where(d => !filterDto.Subspecialties.Any() || filterDto.Subspecialties.Any(ss => ss == d.Subspecialty.DataId))
                            .ToList();

                        var hasAppointmentAvailable = false;

                        foreach (var doctor in doctors)
                        {
                            for (var day = filterDto.AvailableAppointmentStartDate.Value.Date; day <= filterDto.AvailableAppointmentEndDate.Value.Date; day = day.AddDays(1))
                            {
                                if (doctor.GetAllAvailablesForDay(day).Any())
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
                        Ratings = ratings.Select(r => new RatingDto { User = r.User.Email, Score = r.Score, Comment = r.Comment, DateTime = r.DateTime }).ToList(),
                        Specialties = specialties.Select(s => s.Data.Description).ToList(),
                        Subspecialties = subspecialties.Select(sp => sp.Data.Description).ToList(),
                        MedicalInsurances = medicalInsurances.Select(mi => mi.Data.Description).ToList(),
                        MedicalPlans = medicalPlans.Select(mp => mp.Data.Description).ToList(),
                        Logo = clinic.Logo,
                        OpenCloseHours = clinic.OpenCloseHours.Select(och => new OpenCloseHoursDto { DayNumber = och.DayNumber, Start = och.Start, End = och.End }).ToList()
                    });
                }
            }

            return res.OrderByDescending(c => c.Score).ToList();
        }

        [HttpPost]
        [Authorize(Roles = Roles.Client)]
        public bool IsPatientOfClinic([FromBody] IdDto idDto)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

                var clinic = dbContext.Clinics.FirstOrDefault(c => c.Id == idDto.Id);

                if (clinic == null)
                {
                    throw new BadRequestException(ExceptionMessages.BadRequest);
                }

                var client = dbContext.Clinic_Clients.FirstOrDefault(c => c.UserId == userId);

                var patient = client.Patients.FirstOrDefault(p => p.UserId == clinic.UserId);

                return patient != null;
            }
        }

        private int GetUserId()
        {
            int? userId = (int?)HttpContext.Items["userId"];

            if (!userId.HasValue)
            {
                throw new ApplicationException(ExceptionMessages.InternalServerError);
            }

            return userId.Value;
        }
    }
}
