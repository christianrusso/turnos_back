﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaTurnos.Commons.Authorization;
using SistemaTurnos.Commons.Exceptions;
using SistemaTurnos.Database;
using SistemaTurnos.Database.ClinicModel;
using SistemaTurnos.Database.Enums;
using SistemaTurnos.WebApplication.WebApi.Dto;
using SistemaTurnos.WebApplication.WebApi.Dto.Appointment;
using SistemaTurnos.WebApplication.WebApi.Dto.Common;
using SistemaTurnos.WebApplication.WebApi.Dto.Doctor;
using SistemaTurnos.WebApplication.WebApi.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SistemaTurnos.WebApplication.WebApi.Controllers
{
    [Route("Api/[controller]/[action]")]
    [Produces("application/json")]
    [EnableCors("AnyOrigin")]
    [Authorize(Roles = Roles.AdministratorAndEmployeeAndClient)]
    public class DoctorController : Controller
    {
        private BusinessPlaceService _service;

        public DoctorController()
        {
            _service = new BusinessPlaceService();
        }

        /// <summary>
        /// Agrega un medico
        /// </summary>
        [HttpPost]
        public void Add([FromBody] AddDoctorDto doctorDto)
        {
            var watch = Stopwatch.StartNew();

            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _service.GetUserId(HttpContext);

                ValidateDoctorData(dbContext, userId, doctorDto.Subspecialties, doctorDto.WorkingHours);

                var doctor = new Clinic_Doctor
                {
                    FirstName = doctorDto.FirstName,
                    LastName = doctorDto.LastName,
                    Email = doctorDto.Email,
                    PhoneNumber = doctorDto.PhoneNumber,
                    Subspecialties = new List<Clinic_DoctorSubspecialty>(),
                    WorkingHours = new List<Clinic_WorkingHours>(),
                    State = DoctorStateEnum.Active,
                    UserId = userId
                };

                dbContext.Clinic_Doctors.Add(doctor);
                dbContext.SaveChanges();

                doctor.Subspecialties = doctorDto.Subspecialties.Select(sp => new Clinic_DoctorSubspecialty { DoctorId = doctor.Id, SubspecialtyId = sp.SubspecialtyId, ConsultationLength = sp.ConsultationLength }).ToList();
                doctor.WorkingHours = doctorDto.WorkingHours.Select(wh => new Clinic_WorkingHours { DayNumber = wh.DayNumber, Start = wh.Start, End = wh.End }).ToList();
                dbContext.SaveChanges();
            }

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("DoctorController/Add milisegundos: " + elapsedMs);
        }

        /// <summary>
        /// Elimina un medico
        /// </summary>
        [HttpPost]
        public void Remove([FromBody] RemoveDoctorDto doctorDto)
        {
            var watch = Stopwatch.StartNew();

            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _service.GetUserId(HttpContext);
                var doctorToDelete = dbContext.Clinic_Doctors.FirstOrDefault(d => d.Id == doctorDto.Id && d.UserId == userId);

                if (doctorToDelete == null)
                {
                    throw new BadRequestException();
                }

                doctorToDelete.WorkingHours.ForEach(wh => dbContext.Entry(wh).State = EntityState.Deleted);
                doctorToDelete.Appointments.ForEach(a => dbContext.Entry(a).State = EntityState.Deleted);
                dbContext.Entry(doctorToDelete).State = EntityState.Deleted;
                dbContext.SaveChanges();
            }

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("DoctorController/Remove milisegundos: " + elapsedMs);
        }

        /// <summary>
        /// Edita un medico
        /// </summary>
        [HttpPost]
        public void Edit([FromBody] EditDoctorDto doctorDto)
        {
            var watch = Stopwatch.StartNew();

            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _service.GetUserId(HttpContext);

                Clinic_Doctor doctorToUpdate = dbContext.Clinic_Doctors.FirstOrDefault(d => d.Id == doctorDto.Id && d.UserId == userId);

                if (doctorToUpdate == null)
                {
                    throw new BadRequestException();
                }

                ValidateDoctorData(dbContext, userId, doctorDto.Subspecialties, doctorDto.WorkingHours);

                doctorToUpdate.FirstName = doctorDto.FirstName;
                doctorToUpdate.LastName = doctorDto.LastName;
                doctorToUpdate.Email = doctorDto.Email;
                doctorToUpdate.PhoneNumber = doctorDto.PhoneNumber;
                doctorToUpdate.Subspecialties.ForEach(sp => dbContext.Entry(sp).State = EntityState.Deleted);
                doctorToUpdate.WorkingHours.ForEach(wh => dbContext.Entry(wh).State = EntityState.Deleted);

                var newSubspecialties = doctorDto.Subspecialties.Select(sp => new Clinic_DoctorSubspecialty
                {
                    DoctorId = doctorToUpdate.Id,
                    SubspecialtyId = sp.SubspecialtyId,
                    ConsultationLength = sp.ConsultationLength
                }).ToList();

                var newWorkingHours = doctorDto.WorkingHours.Select(wh => new Clinic_WorkingHours
                {
                    DayNumber = wh.DayNumber,
                    Start = wh.Start,
                    End = wh.End
                }).ToList();

                doctorToUpdate.Subspecialties = newSubspecialties;
                doctorToUpdate.WorkingHours = newWorkingHours;

                dbContext.SaveChanges();
            }

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("DoctorController/Edit milisegundos: " + elapsedMs);
        }

        /// <summary>
        /// Devuevle todos los medicos de una clinica logueada
        /// </summary>
        [HttpGet]
        public List<DoctorDto> GetAll()
        {
            var watch = Stopwatch.StartNew();

            var now = DateTime.Now;

            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _service.GetUserId(HttpContext);

                var res = dbContext.Clinic_Doctors
                    .Where(d => d.UserId == userId)
                    .Select(d => new DoctorDto {
                        Id = d.Id,
                        FirstName = d.FirstName,
                        LastName = d.LastName,
                        Email = d.Email,
                        PhoneNumber = d.PhoneNumber,
                        Subspecialties = d.Subspecialties.Select(ssp => new DoctorSubspecialtyInfoDto
                        {
                            SpecialtyId = ssp.Subspecialty.SpecialtyId,
                            SpecialtyDescription = ssp.Subspecialty.Specialty.Data.Description,
                            SubspecialtyId = ssp.SubspecialtyId,
                            SubspecialtyDescription = ssp.Subspecialty.Data.Description,
                            ConsultationLength = ssp.ConsultationLength
                        }).ToList(),
                        State = d.WorkingHours.Any(wh => wh.DayNumber == now.DayOfWeek && wh.Start >= now.TimeOfDay && now.TimeOfDay <= wh.End),
                        WorkingHours = d.WorkingHours.Select(wh => new WorkingHoursDto { DayNumber = wh.DayNumber, Start = wh.Start, End = wh.End }).OrderBy(wh => wh.DayNumber).ToList(),
                        Appointments = d.Appointments.OrderBy(a => a.DateTime).Take(10).Select(a => a.DateTime).ToList()
                    }).ToList();

                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                Console.WriteLine("DoctorController/GetAll milisegundos: " + elapsedMs);

                return res;
            }
        }

        /// <summary>
        /// Devuevle todos los medicos de una clinica logueada para usar en campos SELECT2
        /// </summary>
        [HttpGet]
        public List<SelectOptionDto> GetAllForSelect()
        {
            var watch = Stopwatch.StartNew();

            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _service.GetUserId(HttpContext);

                var res = dbContext.Clinic_Doctors
                    .Where(d => d.UserId == userId)
                    .Select(d => new SelectOptionDto
                    {
                        Id = d.Id.ToString(),
                        Text = $"{d.FirstName} {d.LastName}",
                    })
                    .ToList();

                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                Console.WriteLine("DoctorController/GetAllForSelect milisegundos: " + elapsedMs);

                return res;
            }
        }

        /// <summary>
        /// Devuevle todos los medicos de una clinica logueada filtrando por algunos filtros.
        /// </summary>
        [HttpPost]
        public List<DoctorDto> GetByFilter([FromBody] FilterDoctorDto filter)
        {
            var watch = Stopwatch.StartNew();

            var now = DateTime.Now;

            using (var dbContext = new ApplicationDbContext())
            {
                int? userId = _service.GetUserId(HttpContext);

                if(filter.ClinicId != null)
                    userId = filter.ClinicId;

                var clinic = dbContext.Clinics.FirstOrDefault(c => c.Id == filter.ClinicId);

                if (clinic != null)
                {
                    userId = clinic.UserId;
                }

                var res = dbContext.Clinic_Doctors
                    .Where(d => d.UserId == userId)
                    .Where(d => filter.Id == null || d.Id == filter.Id)
                    .Where(d => filter.FullName == null || $"{d.FirstName} {d.LastName}".ToLower().Contains(filter.FullName.ToLower()) || $"{d.LastName} {d.FirstName}".ToLower().Contains(filter.FullName.ToLower()))
                    .Where(d => filter.SpecialtyId == null || d.Subspecialties.Any(ssp => ssp.Subspecialty.SpecialtyId == filter.SpecialtyId))
                    .Where(d => filter.SubspecialtyId == null || d.Subspecialties.Any(ssp => ssp.SubspecialtyId == filter.SubspecialtyId))
                    .Select(d => new DoctorDto
                    {
                        Id = d.Id,
                        FirstName = d.FirstName,
                        LastName = d.LastName,
                        Email = d.Email,
                        PhoneNumber = d.PhoneNumber,
                        Subspecialties = d.Subspecialties.Select(ssp => new DoctorSubspecialtyInfoDto
                        {
                            SpecialtyId = ssp.Subspecialty.SpecialtyId,
                            SpecialtyDescription = ssp.Subspecialty.Specialty.Data.Description,
                            SubspecialtyId = ssp.SubspecialtyId,
                            SubspecialtyDescription = ssp.Subspecialty.Data.Description,
                            ConsultationLength = ssp.ConsultationLength
                        }).ToList(),
                        State = d.WorkingHours.Any(wh => wh.DayNumber == now.DayOfWeek && wh.Start <= now.TimeOfDay && now.TimeOfDay <= wh.End),
                        WorkingHours = d.WorkingHours.Select(wh => new WorkingHoursDto { DayNumber = wh.DayNumber, Start = wh.Start, End = wh.End }).OrderBy(wh => wh.DayNumber).ToList(),
                        Appointments = d.Appointments.OrderBy(a => a.DateTime).Take(10).Select(a => a.DateTime).ToList()
                    }).ToList();

                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                Console.WriteLine("DoctorController/GetByFilter milisegundos: " + elapsedMs);

                return res;
            }
        }

        /// <summary>
        /// Desabilita un medico
        /// </summary>
        [HttpPost]
        public void Disable([FromBody] EnableDisableDoctorDto doctorDto)
        {
            var watch = Stopwatch.StartNew();

            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _service.GetUserId(HttpContext);

                Clinic_Doctor doctor = dbContext.Clinic_Doctors.FirstOrDefault(d => d.Id == doctorDto.Id && d.UserId == userId);

                if (doctor == null)
                {
                    throw new BadRequestException();
                }

                doctor.State = DoctorStateEnum.Inactive;
                dbContext.SaveChanges();
            }

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("DoctorController/Disable milisegundos: " + elapsedMs);
        }

        /// <summary>
        /// Habilita un medico
        /// </summary>
        [HttpPost]
        public void Enable([FromBody] EnableDisableDoctorDto doctorDto)
        {
            var watch = Stopwatch.StartNew();

            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _service.GetUserId(HttpContext);

                Clinic_Doctor doctor = dbContext.Clinic_Doctors.FirstOrDefault(d => d.Id == doctorDto.Id && d.UserId == userId);

                if (doctor == null)
                {
                    throw new BadRequestException();
                }

                doctor.State = DoctorStateEnum.Active;
                dbContext.SaveChanges();
            }

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("DoctorController/Enable milisegundos: " + elapsedMs);
        }

        /// <summary>
        /// Pone a un medico de vacaciones.
        /// </summary>
        [HttpPost]
        public void Vacation([FromBody] EnableDisableDoctorDto doctorDto)
        {
            var watch = Stopwatch.StartNew();

            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _service.GetUserId(HttpContext);

                var doctor = dbContext.Clinic_Doctors.FirstOrDefault(d => d.Id == doctorDto.Id && d.UserId == userId);

                if (doctor == null)
                {
                    throw new BadRequestException();
                }

                doctor.State = DoctorStateEnum.Vacation;
                dbContext.SaveChanges();
            }

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("DoctorController/Vacation milisegundos: " + elapsedMs);
        }

        [HttpPost]
        public void BlockDay([FromBody] BlockDayDto dto)
        {
            var watch = Stopwatch.StartNew();

            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _service.GetUserId(HttpContext);

                var doctor = dbContext.Clinic_Doctors.FirstOrDefault(d => d.Id == dto.Id && d.UserId == userId);

                if (doctor == null)
                {
                    throw new BadRequestException();
                }

                if (!doctor.Subspecialties.Any(ssp => ssp.SubspecialtyId == dto.SubspecialtyId))
                {
                    throw new BadRequestException();
                }

                doctor.BlockedDays.Add(new Clinic_BlockedDay
                {
                    DateTime = dto.Day,
                    DoctorId = dto.Id,
                    SubspecialtyId = dto.SubspecialtyId
                });

                dbContext.SaveChanges();
            }

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("DoctorController/BlockDay milisegundos: " + elapsedMs);
        }

        [HttpPost]
        public void UnblockDay([FromBody] BlockDayDto dto)
        {
            var watch = Stopwatch.StartNew();

            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _service.GetUserId(HttpContext);

                var doctor = dbContext.Clinic_Doctors.FirstOrDefault(d => d.Id == dto.Id && d.UserId == userId);

                if (doctor == null)
                {
                    throw new BadRequestException();
                }

                if (!doctor.Subspecialties.Any(ssp => ssp.SubspecialtyId == dto.SubspecialtyId))
                {
                    throw new BadRequestException();
                }

                doctor.BlockedDays
                    .Where(bd => bd.SubspecialtyId == dto.SubspecialtyId && bd.SameDay(dto.Day))
                    .ToList()
                    .ForEach(bd => dbContext.Entry(bd).State = EntityState.Deleted);

                dbContext.SaveChanges();
            }

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("DoctorController/UnblockDay milisegundos: " + elapsedMs);
        }

        private void ValidateDoctorData(ApplicationDbContext dbContext, int userId, List<DoctorSubspecialtyDto> subSpecialties, List<WorkingHoursDto> workingHoursDtos)
        {
            if (!subSpecialties.Any())
            {
                throw new BadRequestException();
            }

            foreach (var sp in subSpecialties)
            {
                var subspecialty = dbContext.Clinic_Subspecialties.FirstOrDefault(s => s.Id == sp.SubspecialtyId && s.UserId == userId);

                if (subspecialty == null)
                {
                    throw new BadRequestException();
                }
            }
            
            var workingHours = workingHoursDtos.OrderBy(wh => wh.DayNumber).ThenBy(wh => wh.Start).ToList();

            TimeSpan min = TimeSpan.MinValue;
            uint dayNumber = 0;

            foreach (var wh in workingHours)
            {
                if ((uint)wh.DayNumber > dayNumber)
                {
                    min = TimeSpan.MinValue;
                }

                if (wh.Start <= min || wh.End <= min || wh.End <= wh.Start)
                {
                    throw new BadRequestException();
                }

                min = wh.End;
            }
        }

        /// <summary>
        /// Devuevle todas las subespecialidades bloqueadas para los médicos por día
        /// </summary>
        [HttpPost]
        public List<DoctorSubspecialtyBlockedInfoDto> GetDoctorBlockedSubspecialties([FromBody] BlockDayDto filter)
        {
            var now = DateTime.Now;

            using (var dbContext = new ApplicationDbContext())
            {
                int? userId = _service.GetUserId(HttpContext);

                return dbContext.Clinic_BlockedDays
                    .Where(d => d.DateTime.Date == filter.Day.Date)
                    .Select(d => new DoctorSubspecialtyBlockedInfoDto
                    {
                        SubspecialtyId = d.SubspecialtyId,
                        SubspecialtyDescription = d.Subspecialty.Data.Description,
                        Doctor = d.DoctorId
                    }).ToList();
            }
        }
    }
}
