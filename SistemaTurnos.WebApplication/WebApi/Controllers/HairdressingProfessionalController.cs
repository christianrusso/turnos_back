using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaTurnos.Commons.Authorization;
using SistemaTurnos.Commons.Exceptions;
using SistemaTurnos.Database;
using SistemaTurnos.Database.Enums;
using SistemaTurnos.Database.HairdressingModel;
using SistemaTurnos.WebApplication.WebApi.Dto;
using SistemaTurnos.WebApplication.WebApi.Dto.Appointment;
using SistemaTurnos.WebApplication.WebApi.Dto.Common;
using SistemaTurnos.WebApplication.WebApi.Dto.HairdressingProfessional;
using SistemaTurnos.WebApplication.WebApi.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SistemaTurnos.WebApplication.WebApi.Controllers
{
    [Route("Api/Hairdressing/[controller]/[action]")]
    [Produces("application/json")]
    [EnableCors("AnyOrigin")]
    [Authorize(Roles = Roles.AdministratorAndEmployeeAndClient)]
    public class HairdressingProfessionalController : Controller
    {
        private BusinessPlaceService _service;

        public HairdressingProfessionalController()
        {
            _service = new BusinessPlaceService();
        }

        [HttpPost]
        public void Add([FromBody] AddHairdressingProfessionalDto hairdressingProfessionalDto)
        {
            var watch = Stopwatch.StartNew();

            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _service.GetUserId(HttpContext);

                ValidateHairdressingProfessionalData(dbContext, userId, hairdressingProfessionalDto.Subspecialties, hairdressingProfessionalDto.WorkingHours);

                var HairdressingProfessional = new Hairdressing_Professional
                {
                    FirstName = hairdressingProfessionalDto.FirstName,
                    LastName = hairdressingProfessionalDto.LastName,
                    Email = hairdressingProfessionalDto.Email,
                    PhoneNumber = hairdressingProfessionalDto.PhoneNumber,
                    Subspecialties = new List<Hairdressing_ProfessionalSubspecialty>(),
                    WorkingHours = new List<Hairdressing_WorkingHours>(),
                    State = HairdressingProfessionalStateEnum.Active,
                    UserId = userId
                };

                dbContext.Hairdressing_Professionals.Add(HairdressingProfessional);
                dbContext.SaveChanges();

                HairdressingProfessional.Subspecialties = hairdressingProfessionalDto.Subspecialties.Select(sp => new Hairdressing_ProfessionalSubspecialty
                {
                    ProfessionalId = HairdressingProfessional.Id,
                    SubspecialtyId = sp.SubspecialtyId,
                    ConsultationLength = sp.ConsultationLength
                }).ToList();

                HairdressingProfessional.WorkingHours = hairdressingProfessionalDto.WorkingHours.Select(wh => new Hairdressing_WorkingHours
                {
                    DayNumber = wh.DayNumber,
                    Start = wh.Start,
                    End = wh.End
                }).ToList();
                dbContext.SaveChanges();
            }

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("HairdressingProfessional/Add milisegundos: " + elapsedMs);
        }

        [HttpPost]
        public void Remove([FromBody] RemoveHairdressingProfessionalDto hairdressingProfessionalDto)
        {
            var watch = Stopwatch.StartNew();

            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _service.GetUserId(HttpContext);
                var hairdressingProfessionalToDelete = dbContext.Hairdressing_Professionals.FirstOrDefault(d => d.Id == hairdressingProfessionalDto.Id && d.UserId == userId);

                if (hairdressingProfessionalToDelete == null)
                {
                    throw new BadRequestException();
                }

                hairdressingProfessionalToDelete.WorkingHours.ForEach(wh => dbContext.Entry(wh).State = EntityState.Deleted);
                hairdressingProfessionalToDelete.Appointments.ForEach(a => dbContext.Entry(a).State = EntityState.Deleted);
                dbContext.Entry(hairdressingProfessionalToDelete).State = EntityState.Deleted;
                dbContext.SaveChanges();
            }

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("HairdressingProfessional/Remove milisegundos: " + elapsedMs);
        }

        [HttpPost]
        public void Edit([FromBody] EditHairdressingProfessionalDto hairdressingProfessionalDto)
        {
            var watch = Stopwatch.StartNew();

            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _service.GetUserId(HttpContext);

                Hairdressing_Professional hairdressingProfessionalToUpdate = dbContext.Hairdressing_Professionals.FirstOrDefault(d => d.Id == hairdressingProfessionalDto.Id && d.UserId == userId);

                if (hairdressingProfessionalToUpdate == null)
                {
                    throw new BadRequestException();
                }

                ValidateHairdressingProfessionalData(dbContext, userId, hairdressingProfessionalDto.Subspecialties, hairdressingProfessionalDto.WorkingHours);

                hairdressingProfessionalToUpdate.FirstName = hairdressingProfessionalDto.FirstName;
                hairdressingProfessionalToUpdate.LastName = hairdressingProfessionalDto.LastName;
                hairdressingProfessionalToUpdate.Email = hairdressingProfessionalDto.Email;
                hairdressingProfessionalToUpdate.PhoneNumber = hairdressingProfessionalDto.PhoneNumber;
                hairdressingProfessionalToUpdate.Subspecialties.ForEach(sp => dbContext.Entry(sp).State = EntityState.Deleted);
                hairdressingProfessionalToUpdate.WorkingHours.ForEach(wh => dbContext.Entry(wh).State = EntityState.Deleted);

                var newSubspecialties = hairdressingProfessionalDto.Subspecialties.Select(sp => new Hairdressing_ProfessionalSubspecialty {
                    ProfessionalId = hairdressingProfessionalToUpdate.Id,
                    SubspecialtyId = sp.SubspecialtyId,
                    ConsultationLength = sp.ConsultationLength
                }).ToList();

                var newWorkingHours = hairdressingProfessionalDto.WorkingHours.Select(wh => new Hairdressing_WorkingHours
                {
                    DayNumber = wh.DayNumber,
                    Start = wh.Start,
                    End = wh.End
                }).ToList();

                hairdressingProfessionalToUpdate.Subspecialties = newSubspecialties;
                hairdressingProfessionalToUpdate.WorkingHours = newWorkingHours;

                dbContext.SaveChanges();
            }

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("HairdressingProfessional/Edit milisegundos: " + elapsedMs);
        }

        [HttpGet]
        public List<HairdressingProfessionalDto> GetAll()
        {
            var watch = Stopwatch.StartNew();

            var now = DateTime.Now;

            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _service.GetUserId(HttpContext);

                var res = dbContext.Hairdressing_Professionals
                    .Where(d => d.UserId == userId)
                    .Select(d => new HairdressingProfessionalDto {
                        Id = d.Id,
                        FirstName = d.FirstName,
                        LastName = d.LastName,
                        Email = d.Email,
                        PhoneNumber = d.PhoneNumber,
                        Subspecialties = d.Subspecialties.Select(ssp => new HairdressingProfessionalSubspecialtyInfoDto
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
                Console.WriteLine("HairdressingProfessional/GetAll milisegundos: " + elapsedMs);

                return res;
            }
        }

        [HttpGet]
        public List<SelectOptionDto> GetAllForSelect()
        {
            var watch = Stopwatch.StartNew();

            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _service.GetUserId(HttpContext);

                var res =  dbContext.Hairdressing_Professionals
                    .Where(d => d.UserId == userId)
                    .Select(d => new SelectOptionDto
                    {
                        Id = d.Id.ToString(),
                        Text = $"{d.FirstName} {d.LastName}",
                    })
                    .ToList();

                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                Console.WriteLine("HairdressingProfessional/GetAllForSelect milisegundos: " + elapsedMs);

                return res;
            }
        }

        [HttpPost]
        public List<HairdressingProfessionalDto> GetByFilter([FromBody] FilterHairdressingProfessionalDto filter)
        {
            var watch = Stopwatch.StartNew();

            var now = DateTime.Now;

            using (var dbContext = new ApplicationDbContext())
            {
                int? userId = _service.GetUserId(HttpContext);

                if(filter.HairdressingId != null)
                    userId = filter.HairdressingId;
                
                var hairdressing = dbContext.Hairdressings.FirstOrDefault(c => c.Id == filter.HairdressingId);

                if (hairdressing != null)
                {
                    userId = hairdressing.UserId;
                }
                

                var res = dbContext.Hairdressing_Professionals
                    .Where(d => d.UserId == userId)
                    .Where(d => filter.Id == null || d.Id == filter.Id)
                    .Where(d => filter.FullName == null || $"{d.FirstName} {d.LastName}".ToLower().Contains(filter.FullName.ToLower()) || $"{d.LastName} {d.FirstName}".ToLower().Contains(filter.FullName.ToLower()))
                    .Where(d => filter.SpecialtyId == null || d.Subspecialties.Any(ssp => ssp.Subspecialty.SpecialtyId == filter.SpecialtyId))
                    .Where(d => filter.SubspecialtyId == null || d.Subspecialties.Any(ssp => ssp.SubspecialtyId == filter.SubspecialtyId))
                    .Select(d => new HairdressingProfessionalDto
                    {
                        Id = d.Id,
                        FirstName = d.FirstName,
                        LastName = d.LastName,
                        Email = d.Email,
                        PhoneNumber = d.PhoneNumber,
                        Subspecialties = d.Subspecialties.Select(ssp => new HairdressingProfessionalSubspecialtyInfoDto
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
                Console.WriteLine("HairdressingProfessional/GetByFilter milisegundos: " + elapsedMs);

                return res;
            }
        }

        [HttpPost]
        public void Disable([FromBody] EnableDisableHairdressingProfessionalDto hairdressingProfessionalDto)
        {
            var watch = Stopwatch.StartNew();

            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _service.GetUserId(HttpContext);

                Hairdressing_Professional hairdressingProfessional = dbContext.Hairdressing_Professionals.FirstOrDefault(d => d.Id == hairdressingProfessionalDto.Id && d.UserId == userId);

                if (hairdressingProfessional == null)
                {
                    throw new BadRequestException();
                }

                hairdressingProfessional.State = HairdressingProfessionalStateEnum.Inactive;
                dbContext.SaveChanges();
            }

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("HairdressingProfessional/Disable milisegundos: " + elapsedMs);
        }

        [HttpPost]
        public void Enable([FromBody] EnableDisableHairdressingProfessionalDto hairdressingProfessionalDto)
        {
            var watch = Stopwatch.StartNew();

            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _service.GetUserId(HttpContext);

                Hairdressing_Professional hairdressingProfessional = dbContext.Hairdressing_Professionals.FirstOrDefault(d => d.Id == hairdressingProfessionalDto.Id && d.UserId == userId);

                if (hairdressingProfessional == null)
                {
                    throw new BadRequestException();
                }

                hairdressingProfessional.State = HairdressingProfessionalStateEnum.Active;
                dbContext.SaveChanges();
            }

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("HairdressingProfessional/Enable milisegundos: " + elapsedMs);
        }

        [HttpPost]
        public void Vacation([FromBody] EnableDisableHairdressingProfessionalDto hairdressingProfessionalDto)
        {
            var watch = Stopwatch.StartNew();

            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _service.GetUserId(HttpContext);

                Hairdressing_Professional hairdressingProfessional = dbContext.Hairdressing_Professionals.FirstOrDefault(d => d.Id == hairdressingProfessionalDto.Id && d.UserId == userId);

                if (hairdressingProfessional == null)
                {
                    throw new BadRequestException();
                }

                hairdressingProfessional.State = HairdressingProfessionalStateEnum.Vacation;
                dbContext.SaveChanges();
            }

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("HairdressingProfessional/Vacation milisegundos: " + elapsedMs);
        }

        [HttpPost]
        public void BlockDay([FromBody] BlockDayDto dto)
        {
            var watch = Stopwatch.StartNew();

            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _service.GetUserId(HttpContext);

                var professional = dbContext.Hairdressing_Professionals.FirstOrDefault(d => d.Id == dto.Id && d.UserId == userId);

                if (professional == null)
                {
                    throw new BadRequestException();
                }

                if (!professional.Subspecialties.Any(ssp => ssp.SubspecialtyId == dto.SubspecialtyId))
                {
                    throw new BadRequestException();
                }

                professional.BlockedDays.Add(new Hairdressing_BlockedDay
                {
                    DateTime = dto.Day,
                    ProfessionalId = dto.Id,
                    SubspecialtyId = dto.SubspecialtyId
                });

                dbContext.SaveChanges();
            }

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("HairdressingProfessional/BlockDay milisegundos: " + elapsedMs);
        }

        [HttpPost]
        public void UnblockDay([FromBody] BlockDayDto dto)
        {
            var watch = Stopwatch.StartNew();

            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _service.GetUserId(HttpContext);

                var professional = dbContext.Hairdressing_Professionals.FirstOrDefault(d => d.Id == dto.Id && d.UserId == userId);

                if (professional == null)
                {
                    throw new BadRequestException();
                }

                if (!professional.Subspecialties.Any(ssp => ssp.SubspecialtyId == dto.SubspecialtyId))
                {
                    throw new BadRequestException();
                }

                professional.BlockedDays
                    .Where(bd => bd.SubspecialtyId == dto.SubspecialtyId && bd.SameDay(dto.Day))
                    .ToList()
                    .ForEach(bd => dbContext.Entry(bd).State = EntityState.Deleted);

                dbContext.SaveChanges();
            }

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("HairdressingProfessional/UnblockDay milisegundos: " + elapsedMs);
        }

        private void ValidateHairdressingProfessionalData(ApplicationDbContext dbContext, int userId, List<HairdressingProfessionalSubspecialtyDto> subspecialties, List<WorkingHoursDto> workingHoursDtos)
        {
            if (!subspecialties.Any())
            {
                throw new BadRequestException();
            }

            foreach (var ssp in subspecialties)
            {
                var subspecialty = dbContext.Hairdressing_Subspecialties.FirstOrDefault(s => s.Id == ssp.SubspecialtyId && s.UserId == userId);

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
        public List<HairdressingProfessionalSubspecialtyBlockedInfoDto> GetProfessionalBlockedSubspecialties([FromBody] BlockDayDto filter)
        {
            var watch = Stopwatch.StartNew();

            var now = DateTime.Now;

            using (var dbContext = new ApplicationDbContext())
            {
                int? userId = _service.GetUserId(HttpContext);

                var res = dbContext.Hairdressing_BlockedDays
                    .Where(d => d.DateTime.Date == filter.Day.Date)
                    .Select(d => new HairdressingProfessionalSubspecialtyBlockedInfoDto
                    {
                        SubspecialtyId = d.SubspecialtyId,
                        SubspecialtyDescription = d.Subspecialty.Data.Description,
                        HairdressingProfessional = d.ProfessionalId
                    }).ToList();

                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                Console.WriteLine("HairdressingProfessional/GetProfessionalBlockedSubspecialties milisegundos: " + elapsedMs);

                return res;
            }
        }
    }
}
