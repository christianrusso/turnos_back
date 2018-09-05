using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaTurnos.WebApplication.Database;
using SistemaTurnos.WebApplication.Database.ClinicModel;
using SistemaTurnos.WebApplication.WebApi.Authorization;
using SistemaTurnos.WebApplication.WebApi.Dto;
using SistemaTurnos.WebApplication.WebApi.Dto.MedicalInsurance;
using SistemaTurnos.WebApplication.WebApi.Exceptions;

namespace SistemaTurnos.WebApplication.WebApi.Controllers
{
    [Route("Api/[controller]/[action]")]
    [Produces("application/json")]
    [EnableCors("AnyOrigin")]
    public class MedicalInsuranceController : Controller
    {
        [HttpPost]
        public void Add([FromBody] IdDto medicalInsuranceDto)
        {
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
        }

        [HttpPost]
        public List<MedicalInsuranceDto> GetAllByClinic([FromBody] IdDto idDto)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var clinic = dbContext.Clinics.FirstOrDefault(c => c.Id == idDto.Id);

                if (clinic == null)
                {
                    throw new BadRequestException(ExceptionMessages.BadRequest);
                }

                return dbContext.Clinic_MedicalInsurances
                    .Where(s => s.UserId == clinic.UserId)
                    .Select(s => new MedicalInsuranceDto {
                        Id = s.Id,
                        Description = s.Data.Description
                    }).ToList();
            }
        }

        [HttpGet]
        public List<MedicalInsuranceDto> GetAll()
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

                return dbContext.Clinic_MedicalInsurances
                    .Where(s => s.UserId == userId)
                    .Select(s => new MedicalInsuranceDto
                    {
                        Id = s.Id,
                        Description = s.Data.Description
                    }).ToList();
            }
        }

        [HttpGet]
        public List<SelectOptionDto> GetAllForSelect()
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

                return dbContext.Clinic_MedicalInsurances
                    .Where(s => s.UserId == userId)
                    .Select(s => new SelectOptionDto
                    {
                        Id = s.Id.ToString(),
                        Text = s.Data.Description,
                    })
                    .ToList()
                    .Prepend(new SelectOptionDto { Id = "-1", Text = "Todas" })
                    .ToList();
            }
        }

        [HttpPost]
        public List<MedicalInsuranceDto> GetByLetter([FromBody] GetMedicalInsuranceByLetterDto filter)
        {
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

                return res;
            }
        }

        [HttpPost]
        public List<MedicalInsuranceDto> GetByFilter([FromBody] FilterMedicalInsuranceDto filter)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

                var medicalInsurances = dbContext.Clinic_MedicalInsurances
                    .Where(s => s.UserId == userId)
                    .Where(ssp => ssp.Data.Description.Contains(filter.Description))
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

                return res;
            }
        }

        [HttpPost]
        public void Remove([FromBody] IdDto medicalInsuranceDto)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

                var medicalInsuranceToDelete = dbContext.Clinic_MedicalInsurances.FirstOrDefault(m => m.Id == medicalInsuranceDto.Id && m.UserId == userId);
                

                if (medicalInsuranceToDelete == null)
                {
                    throw new BadRequestException(ExceptionMessages.BadRequest);
                }

                dbContext.Entry(medicalInsuranceToDelete).State = EntityState.Deleted;
                dbContext.SaveChanges();
            }
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
