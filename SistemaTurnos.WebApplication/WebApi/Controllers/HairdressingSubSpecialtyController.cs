using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaTurnos.Database;
using SistemaTurnos.Database.HairdressingModel;
using SistemaTurnos.WebApplication.WebApi.Authorization;
using SistemaTurnos.WebApplication.WebApi.Dto;
using SistemaTurnos.WebApplication.WebApi.Dto.Subspecialty;
using SistemaTurnos.WebApplication.WebApi.Exceptions;
using SistemaTurnos.WebApplication.WebApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SistemaTurnos.WebApplication.WebApi.Controllers
{
    [Route("Api/Hairdressing/[controller]/[action]")]
    [Produces("application/json")]
    [EnableCors("AnyOrigin")]
    [Authorize(Roles = Roles.AdministratorAndEmployee)]
    public class HairdressingSubSpecialtyController : Controller
    {
        public BusinessPlaceService _service;
        private ApplicationDbContext _dbContext;
        public HairdressingSubSpecialtyController(ApplicationDbContext dbContext)
        {
            _service = new BusinessPlaceService(this.HttpContext);
            _dbContext = dbContext;
        }

        [HttpPost]
        public void Add([FromBody] AddSubspecialtyDto subSpecialtyDto)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _service.GetUserId(this.HttpContext);

                var specialty = dbContext.Hairdressing_Specialties.FirstOrDefault(s => s.Id == subSpecialtyDto.SpecialtyId);

                if (specialty == null)
                {
                    throw new BadRequestException(ExceptionMessages.BadRequest);
                }

                dbContext.Hairdressing_Subspecialties.Add(new Hairdressing_Subspecialty
                {
                    DataId = subSpecialtyDto.Id,
                    SpecialtyId = subSpecialtyDto.SpecialtyId,
                    ConsultationLength = subSpecialtyDto.ConsultationLength,
                    UserId = userId
                });

                dbContext.SaveChanges();
            }
        }

        [HttpGet]
        public List<SubspecialtyDto> GetAll()
        {
            using (var dbContext = _dbContext)
            {
                var userId = _service.GetUserId(this.HttpContext);

                var res = dbContext.Hairdressing_Subspecialties
                    .Where(ssp => ssp.UserId == userId)
                    .Select(ssp => new SubspecialtyDto {
                        Id = ssp.Id,
                        Description = ssp.Data.Description,
                        ConsultationLength = ssp.ConsultationLength,
                        SpecialtyId = ssp.SpecialtyId,
                        SpecialtyDescription = ssp.Specialty.Data.Description
                }).ToList();

                return res;
            }
        }

        [HttpPost]
        public List<SubspecialtyDto> GetAllOfSpecialty([FromBody] IdDto specialtyIdDto)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _service.GetUserId(this.HttpContext);

                return dbContext.Hairdressing_Subspecialties
                    .Where(ssp => ssp.SpecialtyId == specialtyIdDto.Id && ssp.UserId == userId)
                    .Select(ssp => new SubspecialtyDto
                    {
                        Id = ssp.Id,
                        Description = ssp.Data.Description,
                        ConsultationLength = ssp.ConsultationLength,
                        SpecialtyId = ssp.SpecialtyId,
                        SpecialtyDescription = ssp.Specialty.Data.Description
                    }).ToList();
            }
        }

        [HttpPost]
        public List<SelectOptionDto> GetAllOfSpecialtyForSelect([FromBody] IdDto specialtyIdDto)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _service.GetUserId(this.HttpContext);

                return dbContext.Hairdressing_Subspecialties
                    .Where(ssp => (specialtyIdDto.Id == -1 || ssp.SpecialtyId == specialtyIdDto.Id) && ssp.UserId == userId)
                    .Select(ssp => new SelectOptionDto
                    {
                        Id = ssp.Id.ToString(),
                        Text = ssp.Data.Description,
                    })
                    .ToList()
                    .Prepend(new SelectOptionDto { Id = "-1", Text = "Todas" })
                    .ToList();
            }
        }

        [HttpPost]
        public void Remove([FromBody] IdDto subSpecialtyDto)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _service.GetUserId(this.HttpContext);

                var subSpecialtyToDelete = dbContext.Hairdressing_Subspecialties.FirstOrDefault(ssp => ssp.Id == subSpecialtyDto.Id && ssp.UserId == userId);

                if (subSpecialtyToDelete == null)
                {
                    throw new BadRequestException(ExceptionMessages.BadRequest);
                }

                dbContext.Entry(subSpecialtyToDelete).State = EntityState.Deleted;
                dbContext.SaveChanges();
            }
        }

        [HttpPost]
        public void Edit([FromBody] EditSubspecialtyDto subSpecialtyDto)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _service.GetUserId(this.HttpContext);

                var subSpecialtyToUpdate = dbContext.Hairdressing_Subspecialties.FirstOrDefault(s => s.Id == subSpecialtyDto.Id && s.UserId == userId);

                if (subSpecialtyToUpdate == null)
                {
                    throw new BadRequestException(ExceptionMessages.BadRequest);
                }

                subSpecialtyToUpdate.ConsultationLength = subSpecialtyDto.ConsultationLength;
                dbContext.SaveChanges();
            }
        }
    }
}
