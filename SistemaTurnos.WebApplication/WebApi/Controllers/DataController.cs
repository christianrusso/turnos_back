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
        public List<SelectOptionDto> GetSubspecialtiesForSelect([FromBody] IdDto specialtyFilter)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var specialty = dbContext.Clinic_Specialties.FirstOrDefault(s => s.Id == specialtyFilter.Id);

                return dbContext.Subspecialties
                    .Where(ssp => ssp.SpecialtyDataId == specialty.Data.Id)
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
        public List<SelectOptionDto> GetMedicalPlansForSelect([FromBody] IdDto medicalInsuranceFilter)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var medicalInsurance = dbContext.Clinic_MedicalInsurances.FirstOrDefault(mi => mi.Id == medicalInsuranceFilter.Id);

                return dbContext.MedicalPlans
                    .Where(mp => mp.MedicalInsuranceDataId == medicalInsurance.Data.Id)
                    .Select(mp => new SelectOptionDto
                    {
                        Id = mp.Id.ToString(),
                        Text = mp.Description
                    })
                    .ToList();
            }
        }
    }
}
