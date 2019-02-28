using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaTurnos.Commons.Exceptions;
using SistemaTurnos.Database;
using SistemaTurnos.Database.ClinicModel;
using SistemaTurnos.WebApplication.WebApi.Dto;
using SistemaTurnos.WebApplication.WebApi.Dto.MedicalInsurance;
using SistemaTurnos.WebApplication.WebApi.Dto.MedicalPlan;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SistemaTurnos.WebApplication.WebApi.Controllers
{
    [Route("Api/[controller]/[action]")]
    [Produces("application/json")]
    [EnableCors("AnyOrigin")]
    public class MedicalPlanController : Controller
    {
        [HttpPost]
        public void AddAll([FromBody] AddMedicalPlansDto addMedicalPlansDto)
        {
            var watch = Stopwatch.StartNew();

            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

                var medicalInsurance = dbContext.Clinic_MedicalInsurances.FirstOrDefault(mi => mi.Id == addMedicalPlansDto.MedicalInsuranceId && mi.UserId == userId);

                if (medicalInsurance == null)
                {
                    throw new BadRequestException();
                }

                medicalInsurance.MedicalPlans.ForEach(mp => dbContext.Entry(mp).State = EntityState.Deleted);
                addMedicalPlansDto.MedicalPlans.ForEach(mpDto => dbContext.Clinic_MedicalPlans.Add(new Clinic_MedicalPlan {
                    DataId = mpDto.Id,
                    MedicalInsuranceId = medicalInsurance.Id,
                    UserId = userId
                }));

                dbContext.SaveChanges();
            }

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("MedicalPlanController/AddAll milisegundos: " + elapsedMs);
        }

        [HttpPost]
        public void Add([FromBody] AddMedicalPlanDto addMedicalPlanDto)
        {
            var watch = Stopwatch.StartNew();

            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

                var medicalInsurance = dbContext.Clinic_MedicalInsurances.FirstOrDefault(mi => mi.Id == addMedicalPlanDto.MedicalInsuranceId && mi.UserId == userId);

                if (medicalInsurance == null)
                {
                    throw new BadRequestException();
                }

                dbContext.Clinic_MedicalPlans.Add(new Clinic_MedicalPlan {
                    DataId = addMedicalPlanDto.Id,
                    MedicalInsuranceId = addMedicalPlanDto.MedicalInsuranceId,
                    UserId = userId
                });

                dbContext.SaveChanges();
            }

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("MedicalPlanController/Add milisegundos: " + elapsedMs);
        }

        [HttpPost]
        public void Remove([FromBody] IdDto idDto)
        {
            var watch = Stopwatch.StartNew();

            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

                var medicalPlan = dbContext.Clinic_MedicalPlans.FirstOrDefault(mp => mp.Id == idDto.Id && mp.UserId == userId);

                if (medicalPlan == null)
                {
                    throw new BadRequestException();
                }

                dbContext.Entry(medicalPlan).State = EntityState.Deleted;

                dbContext.SaveChanges();
            }

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("MedicalPlanController/Remove milisegundos: " + elapsedMs);
        }

        [HttpPost]
        public List<SelectOptionDto> GetAllOfInsuranceForSelect([FromBody] IdDto idDto)
        {
            var watch = Stopwatch.StartNew();

            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

                var res = dbContext.Clinic_MedicalPlans
                    .Where(mp => mp.UserId == userId)
                    .Where(mp => mp.MedicalInsuranceId == idDto.Id)
                    .Select(mp => new SelectOptionDto
                    {
                        Id = mp.Id.ToString(),
                        Text = mp.Data.Description,
                    })
                    .ToList();

                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                Console.WriteLine("MedicalPlanController/Remove milisegundos: " + elapsedMs);

                return res;
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

        [HttpPost]
        public List<MedicalPlanDto> GetAll([FromBody] IdAndUserDto idDto)
        {
            var watch = Stopwatch.StartNew();

            using (var dbContext = new ApplicationDbContext())
            {
                var res = dbContext.Clinic_MedicalPlans
                    .Where(s => s.MedicalInsuranceId == idDto.Id)
                    .Where(s => s.UserId == idDto.UserId)
                    .Select(s => new MedicalPlanDto
                    {
                        Id = s.Id,
                        Description = s.Data.Description
                    }).ToList();

                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                Console.WriteLine("MedicalPlanController/GetAll milisegundos: " + elapsedMs);

                return res;
            }
        }
    }
}
