using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaTurnos.WebApplication.Database;
using SistemaTurnos.WebApplication.Database.ClinicModel;
using SistemaTurnos.WebApplication.Database.Enums;
using SistemaTurnos.WebApplication.WebApi.Authorization;
using SistemaTurnos.WebApplication.WebApi.Dto;
using SistemaTurnos.WebApplication.WebApi.Dto.Doctor;
using SistemaTurnos.WebApplication.WebApi.Exceptions;
using SistemaTurnos.WebApplication.WebApi.Extension;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SistemaTurnos.WebApplication.WebApi.Controllers
{
    [Route("Api/[controller]/[action]")]
    [Produces("application/json")]
    [EnableCors("AnyOrigin")]
    [Authorize(Roles = Roles.AdministratorAndEmployee)]
    public class DoctorController : Controller
    {
        [HttpPost]
        public void Add([FromBody] AddDoctorDto doctorDto)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

                ValidateDoctorData(dbContext, userId, doctorDto.SpecialtyId, doctorDto.SubspecialtyId, doctorDto.WorkingHours);

                var doctor = new Clinic_Doctor
                {
                    FirstName = doctorDto.FirstName,
                    LastName = doctorDto.LastName,
                    Email = doctorDto.Email,
                    PhoneNumber = doctorDto.PhoneNumber,
                    SpecialtyId = doctorDto.SpecialtyId,
                    SubspecialtyId = doctorDto.SubspecialtyId,
                    ConsultationLength = doctorDto.ConsultationLength,
                    WorkingHours = new List<Clinic_WorkingHours>(),
                    State = DoctorStateEnum.Active,
                    UserId = userId
                };

                dbContext.Clinic_Doctors.Add(doctor);
                dbContext.SaveChanges();

                doctor.WorkingHours = doctorDto.WorkingHours.Select(wh => new Clinic_WorkingHours { DayNumber = wh.DayNumber, Start = wh.Start, End = wh.End }).ToList();
                dbContext.SaveChanges();
            }
        }

        [HttpPost]
        public void Remove([FromBody] RemoveDoctorDto doctorDto)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();
                var doctorToDelete = dbContext.Clinic_Doctors.FirstOrDefault(d => d.Id == doctorDto.Id && d.UserId == userId);

                if (doctorToDelete == null)
                {
                    throw new BadRequestException(ExceptionMessages.BadRequest);
                }

                doctorToDelete.WorkingHours.ForEach(wh => dbContext.Entry(wh).State = EntityState.Deleted);
                doctorToDelete.Appointments.ForEach(a => dbContext.Entry(a).State = EntityState.Deleted);
                dbContext.Entry(doctorToDelete).State = EntityState.Deleted;
                dbContext.SaveChanges();
            }
        }

        [HttpPost]
        public void Edit([FromBody] EditDoctorDto doctorDto)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

                Clinic_Doctor doctorToUpdate = dbContext.Clinic_Doctors.FirstOrDefault(d => d.Id == doctorDto.Id && d.UserId == userId);

                if (doctorToUpdate == null)
                {
                    throw new BadRequestException(ExceptionMessages.BadRequest);
                }

                ValidateDoctorData(dbContext, userId, doctorDto.SpecialtyId, doctorDto.SubspecialtyId, doctorDto.WorkingHours);

                doctorToUpdate.FirstName = doctorDto.FirstName;
                doctorToUpdate.LastName = doctorDto.LastName;
                doctorToUpdate.Email = doctorDto.Email;
                doctorToUpdate.PhoneNumber = doctorDto.PhoneNumber;
                doctorToUpdate.SpecialtyId = doctorDto.SpecialtyId;
                doctorToUpdate.SubspecialtyId = doctorDto.SubspecialtyId;
                doctorToUpdate.ConsultationLength = doctorDto.ConsultationLength;
                doctorToUpdate.WorkingHours.ForEach(wh => dbContext.Entry(wh).State = EntityState.Deleted);

                var newWorkingHours = doctorDto.WorkingHours.Select(wh => new Clinic_WorkingHours
                {
                    DayNumber = wh.DayNumber,
                    Start = wh.Start,
                    End = wh.End
                }).ToList();

                doctorToUpdate.WorkingHours = newWorkingHours;

                dbContext.SaveChanges();
            }
        }

        [HttpGet]
        public List<DoctorDto> GetAll()
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

                return dbContext.Clinic_Doctors
                    .Where(d => d.UserId == userId)
                    .Select(d => new DoctorDto {
                        Id = d.Id,
                        FirstName = d.FirstName,
                        LastName = d.LastName,
                        Email = d.Email,
                        PhoneNumber = d.PhoneNumber,
                        SpecialtyId = d.SpecialtyId,
                        SpecialtyDescription = d.Specialty.Data.Description,
                        SubspecialtyId = d.SubspecialtyId,
                        SubspecialtyDescription = d.Subspecialty != null ? d.Subspecialty.Data.Description : "Ninguna",
                        ConsultationLength = d.ConsultationLength,
                        State = d.State.GetEnumDescription(),
                        WorkingHours = d.WorkingHours.Select(wh => new WorkingHoursDto { DayNumber = wh.DayNumber, Start = wh.Start, End = wh.End }).ToList(),
                        Appointments = d.Appointments.OrderBy(a => a.DateTime).Take(10).Select(a => a.DateTime).ToList()
                    }).ToList();
            }
        }

        [HttpGet]
        public List<SelectOptionDto> GetAllForSelect()
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

                return dbContext.Clinic_Doctors
                    .Where(d => d.UserId == userId)
                    .Select(d => new SelectOptionDto
                    {
                        Id = d.Id.ToString(),
                        Text = $"{d.FirstName} {d.LastName}",
                    })
                    .ToList();
            }
        }

        [HttpPost]
        public List<DoctorDto> GetByFilter([FromBody] FilterDoctorDto filter)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

                return dbContext.Clinic_Doctors
                    .Where(d => d.UserId == userId)
                    .Where(d => filter.Id == null || d.Id == filter.Id)
                    .Where(d => filter.FullName == null || $"{d.FirstName} {d.LastName}".Contains(filter.FullName) || $"{d.LastName} {d.FirstName}".Contains(filter.FullName))
                    .Where(d => filter.SpecialtyId == null || d.SpecialtyId == filter.SpecialtyId)
                    .Where(d => filter.SubspecialtyId == null || d.SubspecialtyId == filter.SubspecialtyId)
                    .Select(d => new DoctorDto
                    {
                        Id = d.Id,
                        FirstName = d.FirstName,
                        LastName = d.LastName,
                        Email = d.Email,
                        PhoneNumber = d.PhoneNumber,
                        SpecialtyId = d.SpecialtyId,
                        SpecialtyDescription = d.Specialty.Data.Description,
                        SubspecialtyId = d.SubspecialtyId,
                        SubspecialtyDescription = d.Subspecialty != null ? d.Subspecialty.Data.Description : "Ninguna",
                        ConsultationLength = d.ConsultationLength,
                        State = d.State.GetEnumDescription(),
                        WorkingHours = d.WorkingHours.Select(wh => new WorkingHoursDto { DayNumber = wh.DayNumber, Start = wh.Start, End = wh.End }).ToList(),
                        Appointments = d.Appointments.OrderBy(a => a.DateTime).Take(10).Select(a => a.DateTime).ToList()
                    }).ToList();
            }
        }

        [HttpPost]
        public void Disable([FromBody] EnableDisableDoctorDto doctorDto)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

                Clinic_Doctor doctor = dbContext.Clinic_Doctors.FirstOrDefault(d => d.Id == doctorDto.Id && d.UserId == userId);

                if (doctor == null)
                {
                    throw new BadRequestException(ExceptionMessages.BadRequest);
                }

                doctor.State = DoctorStateEnum.Inactive;
                dbContext.SaveChanges();
            }
        }

        [HttpPost]
        public void Enable([FromBody] EnableDisableDoctorDto doctorDto)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

                Clinic_Doctor doctor = dbContext.Clinic_Doctors.FirstOrDefault(d => d.Id == doctorDto.Id && d.UserId == userId);

                if (doctor == null)
                {
                    throw new BadRequestException(ExceptionMessages.BadRequest);
                }

                doctor.State = DoctorStateEnum.Active;
                dbContext.SaveChanges();
            }
        }

        [HttpPost]
        public void Vacation([FromBody] EnableDisableDoctorDto doctorDto)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

                Clinic_Doctor doctor = dbContext.Clinic_Doctors.FirstOrDefault(d => d.Id == doctorDto.Id && d.UserId == userId);

                if (doctor == null)
                {
                    throw new BadRequestException(ExceptionMessages.BadRequest);
                }

                doctor.State = DoctorStateEnum.Vacation;
                dbContext.SaveChanges();
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

        private void ValidateDoctorData(ApplicationDbContext dbContext, int userId, int specialtyId, int? subSpecialtyId, List<WorkingHoursDto> workingHoursDtos)
        {
            var specialty = dbContext.Clinic_Specialties.FirstOrDefault(s => s.Id == specialtyId && s.UserId == userId);

            if (specialty == null)
            {
                throw new BadRequestException(ExceptionMessages.BadRequest);
            }

            if (subSpecialtyId.HasValue)
            {
                var subspecialty = dbContext.Clinic_Subspecialties.FirstOrDefault(s => s.Id == subSpecialtyId.Value && s.UserId == userId);

                if (subspecialty == null)
                {
                    throw new BadRequestException(ExceptionMessages.BadRequest);
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
                    throw new BadRequestException(ExceptionMessages.BadRequest);
                }

                min = wh.End;
            }
        }
    }
}
