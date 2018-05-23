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
using SistemaTurnos.WebApplication.WebApi.Dto.Specialty;
using SistemaTurnos.WebApplication.WebApi.Dto.Subspecialty;
using SistemaTurnos.WebApplication.WebApi.Exceptions;

namespace SistemaTurnos.WebApplication.WebApi.Controllers
{
    [Route("Api/[controller]/[action]")]
    [Produces("application/json")]
    [EnableCors("AnyOrigin")]
    [Authorize(Roles = Roles.AdministratorAndEmployee)]
    public class SpecialtyController : Controller
    {
        [HttpPost]
        public void Add([FromBody] AddSpecialtyDto specialtyDto)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

                dbContext.Specialties.Add(new Specialty
                {
                    Description = specialtyDto.Description,
                    Subspecialties = new List<Subspecialty>(),
                    UserId = userId
                });

                dbContext.SaveChanges();
            }
        }

        [HttpGet]
        public List<SpecialtyDto> GetAll()
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

                return dbContext.Specialties
                    .Where(s => s.UserId == userId)
                    .Include(s => s.Doctors).Include(s => s.Subspecialties)
                    .ToList()
                    .Select(s => new SpecialtyDto {
                        Id = s.Id,
                        Description = s.Description,
                        Doctors = s.Doctors.Count,
                        Subspecialties = s.Subspecialties.Select(ssp => new SubspecialtyDto
                            {
                                Id = ssp.Id,
                                Description = ssp.Description,
                                ConsultationLength = ssp.ConsultationLength
                            }).ToList()
                    }).ToList();
            }
        }

        [HttpGet]
        public List<SelectOptionDto> GetAllForSelect()
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

                return dbContext.Specialties
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
        public List<SpecialtyDto> GetByLetter([FromBody] GetSubspecialtyByLetterDto filter)
        {
            var firstLetterMinus = char.ToLower(filter.Letter);
            var firstLetterMayus = char.ToUpper(filter.Letter);

            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

                return dbContext.Specialties
                    .Include(s => s.Doctors)
                    .Include(s => s.Subspecialties)
                    .Where(s => s.UserId == userId)
                    .Where(ssp => filter.Letter == '*' || ssp.Description.FirstOrDefault() == firstLetterMinus || ssp.Description.FirstOrDefault() == firstLetterMayus)
                    .ToList()
                    .Select(s => new SpecialtyDto
                    {
                        Id = s.Id,
                        Description = s.Description,
                        Doctors = s.Doctors.Count,
                        Subspecialties = s.Subspecialties.Select(ssp => new SubspecialtyDto
                            {
                                Id = ssp.Id,
                                Description = ssp.Description,
                                ConsultationLength = ssp.ConsultationLength
                            }).ToList()
                    }).ToList();
            }
        }

        [HttpPost]
        public List<SpecialtyDto> GetByFilter([FromBody] FilterSpecialtyDto filter)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

                return dbContext.Specialties
                    .Include(s => s.Doctors)
                    .Include(s => s.Subspecialties)
                    .Where(s => s.UserId == userId)
                    .Where(ssp => ssp.Description.Contains(filter.Description))
                    .ToList()
                    .Select(s => new SpecialtyDto
                    {
                        Id = s.Id,
                        Description = s.Description,
                        Doctors = s.Doctors.Count,
                        Subspecialties = s.Subspecialties.Select(ssp => new SubspecialtyDto
                        {
                            Id = ssp.Id,
                            Description = ssp.Description,
                            ConsultationLength = ssp.ConsultationLength
                        }).ToList()
                    }).ToList();
            }
        }

        [HttpPost]
        public void Remove([FromBody] RemoveSpecialtyDto specialtyDto)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();
                var specialtyToDelete = dbContext.Specialties.Include(s => s.Subspecialties).FirstOrDefaultAsync(s => s.Id == specialtyDto.Id && s.UserId == userId).Result;

                if (specialtyToDelete == null)
                {
                    throw new BadRequestException(ExceptionMessages.BadRequest);
                }

                specialtyToDelete.Subspecialties.ForEach(ssp => dbContext.Entry(ssp).State = EntityState.Deleted);
                dbContext.Entry(specialtyToDelete).State = EntityState.Deleted;
                dbContext.SaveChanges();
            }
        }

        [HttpPost]
        public void Edit([FromBody] EditSpecialtyDto specialtyDto)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

                var specialtyToUpdate = dbContext.Specialties.SingleOrDefaultAsync(s => s.Id == specialtyDto.Id && s.UserId == userId).Result;

                if (specialtyToUpdate == null)
                {
                    throw new BadRequestException(ExceptionMessages.BadRequest);
                }

                specialtyToUpdate.Description = specialtyDto.Description;
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
