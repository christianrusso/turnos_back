using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaTurnos.WebApplication.Database;
using SistemaTurnos.WebApplication.Database.Model;
using SistemaTurnos.WebApplication.WebApi.Authorization;
using SistemaTurnos.WebApplication.WebApi.Dto;
using SistemaTurnos.WebApplication.WebApi.Dto.MedicalInsurance;
using SistemaTurnos.WebApplication.WebApi.Exceptions;

namespace SistemaTurnos.WebApplication.WebApi.Controllers
{
    [Route("Api/[controller]/[action]")]
    [Produces("application/json")]
    [EnableCors("AnyOrigin")]
    [Authorize(Roles = Roles.AdministratorAndEmployee)]
    public class MedicalInsuranceController : Controller
    {
        [HttpPost]
        public void Add([FromBody] AddMedicalInsuranceDto medicalInsuranceDto)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

                dbContext.MedicalInsurances.Add(new MedicalInsurance
                {
                    Description = medicalInsuranceDto.Description,
                    UserId = userId
                });

                dbContext.SaveChanges();
            }
        }

        [HttpGet]
        public List<MedicalInsuranceDto> GetAll()
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

                return dbContext.MedicalInsurances
                    .Where(s => s.UserId == userId)
                    .ToList()
                    .Select(s => new MedicalInsuranceDto {
                        Id = s.Id,
                        Description = s.Description
                    }).ToList();
            }
        }

        [HttpGet]
        public List<SelectOptionDto> GetAllForSelect()
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

                return dbContext.MedicalInsurances
                    .Where(s => s.UserId == userId)
                    .ToList()
                    .Select(s => new SelectOptionDto
                    {
                        Id = s.Id.ToString(),
                        Text = s.Description,
                    })
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

                return dbContext.MedicalInsurances
                    .Include(s => s.MedicalPlans)
                    .Where(s => s.UserId == userId)
                    .Where(ssp => filter.Letter == '*' || ssp.Description.FirstOrDefault() == firstLetterMinus || ssp.Description.FirstOrDefault() == firstLetterMayus)
                    .OrderBy(s => s.Description)
                    .ToList()
                    .Select(s => new MedicalInsuranceDto
                    {
                        Id = s.Id,
                        Description = s.Description,
                        MedicalPlans = s.MedicalPlans.Select(mp => new MedicalPlanDto { Id = mp.Id, Description = mp.Description }).ToList()
                    }).ToList();
            }
        }

        [HttpPost]
        public List<MedicalInsuranceDto> GetByFilter([FromBody] FilterMedicalInsuranceDto filter)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

                return dbContext.MedicalInsurances
                    .Include(s => s.MedicalPlans)
                    .Where(s => s.UserId == userId)
                    .Where(ssp => ssp.Description.Contains(filter.Description))
                    .ToList()
                    .Select(s => new MedicalInsuranceDto
                    {
                        Id = s.Id,
                        Description = s.Description,
                        MedicalPlans = s.MedicalPlans.Select(mp => new MedicalPlanDto { Id = mp.Id, Description = mp.Description }).ToList()
                    }).ToList();
            }
        }

        [HttpPost]
        public void Remove([FromBody] RemoveMedicalInsuranceDto medicalInsuranceDto)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

                var medicalInsuranceToDelete = dbContext.MedicalInsurances
                    .FirstOrDefaultAsync(m => m.Id == medicalInsuranceDto.Id && m.UserId == userId).Result;
                

                if (medicalInsuranceToDelete == null)
                {
                    throw new BadRequestException(ExceptionMessages.BadRequest);
                }

                dbContext.Entry(medicalInsuranceToDelete).State = EntityState.Deleted;
                dbContext.SaveChanges();
            }
        }

        [HttpPost]
        public void Edit([FromBody] EditMedicalInsuranceDto medicalInsuranceDto)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

                var medicalInsuranceToUpdate = dbContext.MedicalInsurances.SingleOrDefaultAsync(m => m.Id == medicalInsuranceDto.Id && m.UserId == userId).Result;

                if (medicalInsuranceDto == null)
                {
                    throw new BadRequestException(ExceptionMessages.BadRequest);
                }

                medicalInsuranceToUpdate.Description = medicalInsuranceDto.Description;
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
