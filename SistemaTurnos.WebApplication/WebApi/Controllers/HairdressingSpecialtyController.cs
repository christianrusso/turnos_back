using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaTurnos.Commons.Exceptions;
using SistemaTurnos.Database;
using SistemaTurnos.Database.HairdressingModel;
using SistemaTurnos.WebApplication.WebApi.Dto;
using SistemaTurnos.WebApplication.WebApi.Dto.Specialty;
using SistemaTurnos.WebApplication.WebApi.Dto.Subspecialty;
using SistemaTurnos.WebApplication.WebApi.Services;

namespace SistemaTurnos.WebApplication.WebApi.Controllers
{
    [Route("Api/Hairdressing/[controller]/[action]")]
    [Produces("application/json")]
    [EnableCors("AnyOrigin")]
    public class HairdressingSpecialtyController : Controller
    {
        private BusinessPlaceService _service;

        public HairdressingSpecialtyController()
        {
            _service = new BusinessPlaceService();
        }

        [HttpPost]
        public void Add([FromBody] IdDto specialtyDto)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _service.GetUserId(this.HttpContext);

                dbContext.Hairdressing_Specialties.Add(new Hairdressing_Specialty
                {
                    DataId = specialtyDto.Id,
                    Subspecialties = new List<Hairdressing_Subspecialty>(),
                    UserId = userId
                });

                dbContext.SaveChanges();
            }
        }

        [HttpGet]
        public List<HairdressingSpecialtyDto> GetAll()
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _service.GetUserId(HttpContext);

                var professionals = dbContext.Hairdressing_Professionals.Where(p => p.UserId == userId);

                return dbContext.Hairdressing_Specialties
                    .Where(s => s.UserId == userId)
                    .Select(s => new HairdressingSpecialtyDto
                    {
                        Id = s.Id,
                        Description = s.Data.Description,
                        Doctors = professionals.Count(p => p.Subspecialties.Any(ssp => ssp.Subspecialty.SpecialtyId == s.Id)),
                        Subspecialties = s.Subspecialties
                            .Select(ssp => new HairdressingSubspecialtyDto
                            {
                                Id = ssp.Id,
                                Description = ssp.Data.Description,
                                ConsultationLength = ssp.ConsultationLength,
                                Price = ssp.Price
                            })
                            .ToList()
                    })
                    .ToList();
            }
        }

        [HttpPost]
        public List<HairdressingSpecialtyDto> GetAllByHairdressing([FromBody] IdDto idDto)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var hairdressings = dbContext.Hairdressings.FirstOrDefault(c => c.Id == idDto.Id);

                if (hairdressings == null)
                {
                    throw new BadRequestException();
                }

                var professionals = dbContext.Hairdressing_Professionals.Where(p => p.UserId == hairdressings.UserId);

                return dbContext.Hairdressing_Specialties
                    .Where(s => s.UserId == hairdressings.UserId)
                    .Select(s => new HairdressingSpecialtyDto
                    {
                        Id = s.Id,
                        Description = s.Data.Description,
                        Doctors = professionals.Count(p => p.Subspecialties.Any(ssp => ssp.Subspecialty.SpecialtyId == s.Id)),
                        Subspecialties = s.Subspecialties
                            .Select(ssp => new HairdressingSubspecialtyDto
                            {
                                Id = ssp.Id,
                                Description = ssp.Data.Description,
                                ConsultationLength = ssp.ConsultationLength,
                                Price = ssp.Price
                            })
                            .ToList()
                    })
                    .ToList();
            }
        }

        [HttpGet]
        public List<SelectOptionDto> GetAllForSelect()
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _service.GetUserId(this.HttpContext);

                return dbContext.Hairdressing_Specialties
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
        public List<HairdressingSpecialtyDto> GetByLetter([FromBody] GetSubspecialtyByLetterDto filter)
        {
            var firstLetterMinus = char.ToLower(filter.Letter);
            var firstLetterMayus = char.ToUpper(filter.Letter);

            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _service.GetUserId(HttpContext);

                var subspecialties = dbContext.Hairdressing_Subspecialties.Where(ssp => ssp.UserId == userId);
                var professionals = dbContext.Hairdressing_Professionals.Where(p => p.UserId == userId);

                return dbContext.Hairdressing_Specialties
                    .Where(s => s.UserId == userId)
                    .Where(ssp => filter.Letter == '*' || ssp.Data.Description.FirstOrDefault() == firstLetterMinus || ssp.Data.Description.FirstOrDefault() == firstLetterMayus)
                    .OrderBy(s => s.Data.Description)
                    .Select(s => new HairdressingSpecialtyDto
                    {
                        Id = s.Id,
                        Description = s.Data.Description,
                        Doctors = professionals.Count(p => p.Subspecialties.Any(ssp => ssp.Subspecialty.SpecialtyId == s.Id)),
                        Subspecialties = subspecialties.Where(ssp => ssp.SpecialtyId == s.Id).Select(ssp => new HairdressingSubspecialtyDto
                        {
                                Id = ssp.Id,
                                Description = ssp.Data.Description,
                                ConsultationLength = ssp.ConsultationLength,
                                Price = ssp.Price
                            }).ToList()
                    }).ToList();
            }
        }

        [HttpPost]
        public List<HairdressingSpecialtyDto> GetByFilter([FromBody] FilterSpecialtyDto filter)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _service.GetUserId(HttpContext);

                var subspecialties = dbContext.Hairdressing_Subspecialties.Where(ssp => ssp.UserId == userId);
                var professionals = dbContext.Hairdressing_Professionals.Where(p => p.UserId == userId);

                return dbContext.Hairdressing_Specialties
                    .Where(s => s.UserId == userId)
                    .Where(ssp => ssp.Data.Description.Contains(filter.Description))
                    .Select(s => new HairdressingSpecialtyDto
                    {
                        Id = s.Id,
                        Description = s.Data.Description,
                        Doctors = professionals.Count(p => p.Subspecialties.Any(ssp => ssp.Subspecialty.SpecialtyId == s.Id)),
                        Subspecialties = subspecialties.Where(ssp => ssp.SpecialtyId == s.Id).Select(ssp => new HairdressingSubspecialtyDto
                        {
                            Id = ssp.Id,
                            Description = ssp.Data.Description,
                            ConsultationLength = ssp.ConsultationLength,
                            Price = ssp.Price
                        }).ToList()
                    }).ToList();
            }
        }

        [HttpPost]
        public void Remove([FromBody] IdDto specialtyDto)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _service.GetUserId(HttpContext);
                var specialtyToDelete = dbContext.Hairdressing_Specialties.FirstOrDefault(s => s.Id == specialtyDto.Id && s.UserId == userId);

                if (specialtyToDelete == null)
                {
                    throw new BadRequestException();
                }

                specialtyToDelete.Subspecialties.ForEach(ssp => dbContext.Entry(ssp).State = EntityState.Deleted);
                dbContext.Entry(specialtyToDelete).State = EntityState.Deleted;
                dbContext.SaveChanges();
            }
        }
    }
}
