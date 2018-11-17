using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using SistemaTurnos.Database;
using SistemaTurnos.WebApplication.WebApi.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using SistemaTurnos.WebApplication.WebApi.Services;
using SistemaTurnos.Database.Enums;
using SistemaTurnos.Commons.Exceptions;
using SistemaTurnos.Commons.Authorization;

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
            _service = new BusinessPlaceService(HttpContext);
        }

        /// <summary>
        /// Devuelve todas las Especialidades de data segun el id de rubro dado.
        /// </summary>
        [HttpPost]
        public List<SelectOptionDto> GetSpecialtiesForSelect([FromBody] IdDto idDto)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                RubroEnum rubro = RubroEnumHelper.GetRubro(idDto.Id);
                
                return dbContext.Specialties
                    .Where(s => s.Rubro == rubro)
                    .Select(s => new SelectOptionDto
                    {
                        Id = s.Id.ToString(),
                        Text = s.Description
                    })
                    .ToList();
            }
        }

        /// <summary>
        /// Devuelve todas las subespeciadlaides de data segun Rubro y especialidad dada.
        /// </summary>
        [HttpPost]
        public List<SelectOptionDto> GetSubspecialtiesForSelect([FromBody] OptionalIdDtoAndRubro filter)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                RubroEnum rubro = RubroEnumHelper.GetRubro(filter.Rubro);

                return dbContext.Subspecialties
                    .Where(ssp => !filter.Id.HasValue || ssp.SpecialtyDataId == filter.Id)
                    .Where(x => x.Rubro == rubro)
                    .Select(s => new SelectOptionDto
                    {
                        Id = s.Id.ToString(),
                        Text = s.Description
                    })
                    .ToList();
            }
        }

        /// <summary>
        /// Devuelve todas las subespecialdiades asociadas a una clinica y de una especiadliadd dada.
        /// </summary>
        [HttpPost]
        [Authorize(Roles = Roles.AdministratorAndEmployee)]
        public List<SelectOptionDto> GetSubspecialtiesByClinicForSelect([FromBody] IdDto specialtyFilter)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _service.GetUserId(HttpContext);

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

        /// <summary>
        /// Devuelve todas las subespecialdiades asociadas a una peluqueria y de una especiadliadd dada.
        /// </summary>
        [HttpPost]
        [Authorize(Roles = Roles.AdministratorAndEmployee)]
        public List<SelectOptionDto> GetSubspecialtiesByHairdressingForSelect([FromBody] IdDto specialtyFilter)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _service.GetUserId(HttpContext);

                var specialty = dbContext.Hairdressing_Specialties.FirstOrDefault(s => s.Id == specialtyFilter.Id && s.UserId == userId);

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

        /// <summary>
        /// Devuelve las obras sociales de data.
        /// </summary>
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

        /// <summary>
        /// Devuelve los planes de obras sociales segun la obra social dada.
        /// </summary>
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

        /// <summary>
        /// Deveulve los planes de una clinica dada con una obra social dada.
        /// </summary>
        [HttpPost]
        [Authorize(Roles = Roles.AdministratorAndEmployee)]
        public List<SelectOptionDto> GetMedicalPlansByClinicForSelect([FromBody] IdDto medicalInsuranceFilter)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _service.GetUserId(HttpContext);

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

        /// <summary>
        /// Devuelve las ciudades de data.
        /// </summary>
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
