﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using SistemaTurnos.WebApplication.Database;
using SistemaTurnos.WebApplication.WebApi.Authorization;
using SistemaTurnos.WebApplication.WebApi.Dto;
using SistemaTurnos.WebApplication.WebApi.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SistemaTurnos.WebApplication.WebApi.Controllers
{
    [Route("Api/[controller]/[action]")]
    [Produces("application/json")]
    [EnableCors("AnyOrigin")]
    public class DataController : Controller
    {
        [HttpPost]
        public List<SelectOptionDto> GetSpecialtiesForSelect()
        {
            using (var dbContext = new ApplicationDbContext())
            {
                return dbContext.Specialties
                    .Select(s => new SelectOptionDto
                    {
                        Id = s.Id.ToString(),
                        Text = s.Description
                    })
                    .ToList();
            }
        }

        [HttpPost]
        public List<SelectOptionDto> GetSubspecialtiesForSelect([FromBody] OptionalIdDto specialtyFilter)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                return dbContext.Subspecialties
                    .Where(ssp => !specialtyFilter.Id.HasValue || ssp.SpecialtyDataId == specialtyFilter.Id)
                    .Select(s => new SelectOptionDto
                    {
                        Id = s.Id.ToString(),
                        Text = s.Description
                    })
                    .ToList();
            }
        }

        [HttpPost]
        [Authorize(Roles = Roles.AdministratorAndEmployee)]
        public List<SelectOptionDto> GetSubspecialtiesByClinicForSelect([FromBody] IdDto specialtyFilter)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

                var specialty = dbContext.Clinic_Specialties.FirstOrDefault(s => s.Id == specialtyFilter.Id && s.UserId == userId);

                if (specialty == null)
                {
                    throw new ApplicationException(ExceptionMessages.BadRequest);
                }

                return dbContext.Subspecialties
                    .Where(ssp => ssp.SpecialtyDataId == specialty.DataId)
                    .Select(s => new SelectOptionDto
                    {
                        Id = s.Id.ToString(),
                        Text = s.Description
                    })
                    .ToList();
            }
        }

        [HttpPost]
        public List<SelectOptionDto> GetMedicalInsurancesForSelect()
        {
            using (var dbContext = new ApplicationDbContext())
            {
                return dbContext.MedicalInsurances
                    .Select(mi => new SelectOptionDto
                    {
                        Id = mi.Id.ToString(),
                        Text = mi.Description
                    })
                    .ToList();
            }
        }

        [HttpPost]
        public List<SelectOptionDto> GetMedicalPlansForSelect([FromBody] OptionalIdDto medicalInsuranceFilter)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                return dbContext.MedicalPlans
                    .Where(mp => medicalInsuranceFilter.Id.HasValue || mp.MedicalInsuranceDataId == medicalInsuranceFilter.Id)
                    .Select(mp => new SelectOptionDto
                    {
                        Id = mp.Id.ToString(),
                        Text = mp.Description
                    })
                    .ToList();
            }
        }

        [HttpPost]
        [Authorize(Roles = Roles.AdministratorAndEmployee)]
        public List<SelectOptionDto> GetMedicalPlansByClinicForSelect([FromBody] IdDto medicalInsuranceFilter)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

                var medicalInsurance = dbContext.Clinic_MedicalInsurances.FirstOrDefault(mi => mi.Id == medicalInsuranceFilter.Id && mi.UserId == userId);

                if (medicalInsurance == null)
                {
                    throw new ApplicationException(ExceptionMessages.BadRequest);
                }

                return dbContext.MedicalPlans
                    .Where(mp => mp.MedicalInsuranceDataId == medicalInsurance.DataId)
                    .Select(mp => new SelectOptionDto
                    {
                        Id = mp.Id.ToString(),
                        Text = mp.Description
                    })
                    .ToList();
            }
        }

        [HttpPost]
        public List<SelectOptionDto> GetCitiesForSelect()
        {
            using (var dbContext = new ApplicationDbContext())
            {
                return dbContext.Cities.Select(c => new SelectOptionDto
                {
                    Id = c.Id.ToString(),
                    Text = c.Name
                }).ToList();
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
