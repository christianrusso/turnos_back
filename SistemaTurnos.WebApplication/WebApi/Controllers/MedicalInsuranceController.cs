using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaTurnos.Commons.Authorization;
using SistemaTurnos.Commons.Exceptions;
using SistemaTurnos.Database;
using SistemaTurnos.Database.ClinicModel;
using SistemaTurnos.WebApplication.WebApi.Dto;
using SistemaTurnos.WebApplication.WebApi.Dto.MedicalInsurance;

namespace SistemaTurnos.WebApplication.WebApi.Controllers
{
    [Route("Api/[controller]/[action]")]
    [Produces("application/json")]
    [EnableCors("AnyOrigin")]
    [Authorize(Roles = Roles.AdministratorAndEmployeeAndClient)]
    public class MedicalInsuranceController : Controller
    {
        [HttpPost]
        public void Add([FromBody] IdDto medicalInsuranceDto)
        {
            var watch = Stopwatch.StartNew();

            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

                dbContext.Clinic_MedicalInsurances.Add(new Clinic_MedicalInsurance
                {
                    DataId = medicalInsuranceDto.Id,
                    UserId = userId
                });

                dbContext.SaveChanges();
            }

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("MedicalInsuranceController/Add milisegundos: " + elapsedMs);
        }

        [HttpPost]
        public List<MedicalInsuranceDto> GetAllByClinic([FromBody] IdDto idDto)
        {
            var watch = Stopwatch.StartNew();

            using (var dbContext = new ApplicationDbContext())
            {
                var clinic = dbContext.Clinics.FirstOrDefault(c => c.Id == idDto.Id);

                if (clinic == null)
                {
                    throw new BadRequestException();
                }

                var res = dbContext.Clinic_MedicalInsurances
                    .Where(s => s.UserId == clinic.UserId)
                    .Select(s => new MedicalInsuranceDto {
                        Id = s.Id,
                        Description = s.Data.Description
                    }).ToList();

                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                Console.WriteLine("MedicalInsuranceController/GetAllByClinic milisegundos: " + elapsedMs);

                return res;
            }
        }

        [HttpGet]
        public List<MedicalInsuranceDto> GetAll()
        {
            var watch = Stopwatch.StartNew();

            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

                var res = dbContext.Clinic_MedicalInsurances
                    .Where(s => s.UserId == userId)
                    .Select(s => new MedicalInsuranceDto
                    {
                        Id = s.Id,
                        Description = s.Data.Description
                    }).ToList();

                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                Console.WriteLine("MedicalInsuranceController/GetAll milisegundos: " + elapsedMs);

                return res;
            }
        }

        [HttpGet]
        public List<SelectOptionDto> GetAllForSelect()
        {
            var watch = Stopwatch.StartNew();

            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

                var res = dbContext.Clinic_MedicalInsurances
                    .Where(s => s.UserId == userId)
                    .Select(s => new SelectOptionDto
                    {
                        Id = s.Id.ToString(),
                        Text = s.Data.Description,
                    })
                    .ToList()
                    .Prepend(new SelectOptionDto { Id = "-1", Text = "Todas" })
                    .ToList();

                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                Console.WriteLine("MedicalInsuranceController/GetAllForSelect milisegundos: " + elapsedMs);

                return res;
            }
        }

        [HttpPost]
        public List<MedicalInsuranceDto> GetByLetter([FromBody] GetMedicalInsuranceByLetterDto filter)
        {
            var watch = Stopwatch.StartNew();

            var firstLetterMinus = char.ToLower(filter.Letter);
            var firstLetterMayus = char.ToUpper(filter.Letter);

            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

                var medicalInsurances = dbContext.Clinic_MedicalInsurances
                    .Where(s => s.UserId == userId)
                    .Where(ssp => filter.Letter == '*' || ssp.Data.Description.FirstOrDefault() == firstLetterMinus || ssp.Data.Description.FirstOrDefault() == firstLetterMayus)
                    .OrderBy(s => s.Data.Description)
                    .ToList();

                var res = new List<MedicalInsuranceDto>();

                foreach (var mi in medicalInsurances)
                {
                    res.Add(new MedicalInsuranceDto
                    {
                        Id = mi.Id,
                        Description = mi.Data.Description,
                        MedicalPlans = dbContext.Clinic_MedicalPlans
                            .Where(mp => mp.MedicalInsuranceId == mi.Id)
                            .Select(mp => new MedicalPlanDto { Id = mp.Id, Description = mp.Data.Description })
                            .ToList()
                    });
                }

                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                Console.WriteLine("MedicalInsuranceController/GetByLetter milisegundos: " + elapsedMs);

                return res;
            }
        }

        [HttpPost]
        public List<MedicalInsuranceDto> GetByFilter([FromBody] FilterMedicalInsuranceDto filter)
        {
            var watch = Stopwatch.StartNew();

            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

                var medicalInsurances = dbContext.Clinic_MedicalInsurances
                    .Where(s => s.UserId == userId)
                    .Where(ssp => ssp.Data.Description.ToLower().Contains(filter.Description.ToLower()))
                    .ToList();

                var res = new List<MedicalInsuranceDto>();

                foreach (var mi in medicalInsurances)
                {
                    res.Add(new MedicalInsuranceDto
                    {
                        Id = mi.Id,
                        Description = mi.Data.Description,
                        MedicalPlans = dbContext.Clinic_MedicalPlans
                            .Where(mp => mp.MedicalInsuranceId == mi.Id)
                            .Select(mp => new MedicalPlanDto { Id = mp.Id, Description = mp.Data.Description })
                            .ToList()
                    });
                }

                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                Console.WriteLine("MedicalInsuranceController/GetByFilter milisegundos: " + elapsedMs);

                return res;
            }
        }

        [HttpPost]
        public void Remove([FromBody] IdDto medicalInsuranceDto)
        {
            var watch = Stopwatch.StartNew();

            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

                var medicalInsuranceToDelete = dbContext.Clinic_MedicalInsurances.FirstOrDefault(m => m.Id == medicalInsuranceDto.Id && m.UserId == userId);
                

                if (medicalInsuranceToDelete == null)
                {
                    throw new BadRequestException();
                }

                dbContext.Entry(medicalInsuranceToDelete).State = EntityState.Deleted;
                dbContext.SaveChanges();
            }

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("MedicalInsuranceController/Remove milisegundos: " + elapsedMs);
        }

        private int GetUserId()
        {
            int? userId = (int?) HttpContext.Items["userId"];

            if (!userId.HasValue)
            {
                throw new ApplicationException(ExceptionMessages.InternalServerError);
            }

            return userId.Value;
        }
    }
}
