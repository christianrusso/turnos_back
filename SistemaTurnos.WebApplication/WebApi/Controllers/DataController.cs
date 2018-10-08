using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using SistemaTurnos.WebApplication.Database;
using SistemaTurnos.WebApplication.WebApi.Authorization;
using SistemaTurnos.WebApplication.WebApi.Dto;
using SistemaTurnos.WebApplication.WebApi.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using SistemaTurnos.WebApplication.WebApi.Services;
using SistemaTurnos.WebApplication.Database.Enums;

namespace SistemaTurnos.WebApplication.WebApi.Controllers
{
    
    [Route("Api/[controller]/[action]")]
    [Produces("application/json")]
    [EnableCors("AnyOrigin")]
    
    public class DataController : Controller
    {

        private BusinessPlaceService _service;

        public DataController()
        {
            _service = new BusinessPlaceService(this.HttpContext);
        }

        
        [HttpPost]
        public List<SelectOptionDto> GetSpecialtiesForSelect([FromBody] IdDto rubro)
        {
            using (var dbContext = new ApplicationDbContext())
            {
            
                RubroEnum rubroEnum = RubroEnum.Clinic;
                
                if(rubro.Id == 2)
                {
                    rubroEnum =  RubroEnum.Hairdressing;
                }
                
                return dbContext.Specialties
                    .Where(x => x.Rubro == rubroEnum)
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
                var userId = _service.GetUserId(this.HttpContext);

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
                var userId = _service.GetUserId(this.HttpContext);

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
    }
}
