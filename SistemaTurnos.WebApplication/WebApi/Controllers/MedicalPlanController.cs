﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaTurnos.WebApplication.Database;
using SistemaTurnos.WebApplication.Database.Model;
using SistemaTurnos.WebApplication.WebApi.Authorization;
using SistemaTurnos.WebApplication.WebApi.Dto;
using SistemaTurnos.WebApplication.WebApi.Dto.MedicalPlan;
using SistemaTurnos.WebApplication.WebApi.Exceptions;
using System;
using System.Linq;

namespace SistemaTurnos.WebApplication.WebApi.Controllers
{
    [Route("Api/[controller]/[action]")]
    [Produces("application/json")]
    [EnableCors("AnyOrigin")]
    [Authorize(Roles = Roles.AdministratorAndEmployee)]
    public class MedicalPlanController : Controller
    {
        [HttpPost]
        public void AddAll([FromBody] AddMedicalPlansDto addMedicalPlansDto)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

                var medicalInsurance = dbContext.MedicalInsurances.Include(mi => mi.MedicalPlans).FirstOrDefault(mi => mi.Id == addMedicalPlansDto.MedicalInsuranceId && mi.UserId == userId);

                if (medicalInsurance == null)
                {
                    throw new ApplicationException(ExceptionMessages.BadRequest);
                }

                medicalInsurance.MedicalPlans.ForEach(mp => dbContext.Entry(mp).State = EntityState.Deleted);
                addMedicalPlansDto.MedicalPlans.ForEach(mpDto => dbContext.MedicalPlans.Add(new MedicalPlan {
                    Description = mpDto.Description,
                    MedicalInsuranceId = medicalInsurance.Id,
                    UserId = userId
                }));

                dbContext.SaveChanges();
            }
        }

        [HttpPost]
        public void Add([FromBody] AddMedicalPlanDto addMedicalPlanDto)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

                var medicalInsurance = dbContext.MedicalInsurances.Include(mi => mi.MedicalPlans).FirstOrDefault(mi => mi.Id == addMedicalPlanDto.MedicalInsuranceId && mi.UserId == userId);

                if (medicalInsurance == null)
                {
                    throw new ApplicationException(ExceptionMessages.BadRequest);
                }

                dbContext.MedicalPlans.Add(new MedicalPlan {
                    Description = addMedicalPlanDto.Description,
                    MedicalInsuranceId = addMedicalPlanDto.MedicalInsuranceId,
                    UserId = userId
                });

                dbContext.SaveChanges();
            }
        }

        [HttpPost]
        public void Remove([FromBody] IdDto idDto)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

                var medicalPlan = dbContext.MedicalPlans.FirstOrDefault(mp => mp.Id == idDto.Id && mp.UserId == userId);

                if (medicalPlan == null)
                {
                    throw new ApplicationException(ExceptionMessages.BadRequest);
                }

                dbContext.Entry(medicalPlan).State = EntityState.Deleted;

                dbContext.SaveChanges();
            }
        }

        [HttpPost]
        public void Edit([FromBody] EditMedicalPlanDto editMedicalPlanDto)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

                var medicalPlan = dbContext.MedicalPlans.FirstOrDefault(mp => mp.Id == editMedicalPlanDto.Id && mp.UserId == userId);

                if (medicalPlan == null)
                {
                    throw new ApplicationException(ExceptionMessages.BadRequest);
                }

                medicalPlan.Description = editMedicalPlanDto.Description;

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
    }
}
