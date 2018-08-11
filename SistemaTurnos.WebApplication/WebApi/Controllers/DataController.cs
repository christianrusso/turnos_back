using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using SistemaTurnos.WebApplication.Database;
using SistemaTurnos.WebApplication.WebApi.Dto;
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
