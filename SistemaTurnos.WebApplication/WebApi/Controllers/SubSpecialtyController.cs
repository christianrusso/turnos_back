using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaTurnos.WebApplication.Database;
using SistemaTurnos.WebApplication.Database.ClinicModel;
using SistemaTurnos.WebApplication.WebApi.Authorization;
using SistemaTurnos.WebApplication.WebApi.Dto;
using SistemaTurnos.WebApplication.WebApi.Dto.Subspecialty;
using SistemaTurnos.WebApplication.WebApi.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SistemaTurnos.WebApplication.WebApi.Controllers
{
    [Route("Api/[controller]/[action]")]
    [Produces("application/json")]
    [EnableCors("AnyOrigin")]
    public class SubspecialtyController : Controller
    {
        /// <summary>
        /// Agrega una subespecialidad a una clinica dada.
        /// </summary>
        [HttpPost]
        public void Add([FromBody] AddSubspecialtyDto subSpecialtyDto)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

                var specialty = dbContext.Clinic_Specialties.FirstOrDefault(s => s.Id == subSpecialtyDto.SpecialtyId);

                if (specialty == null)
                {
                    throw new BadRequestException(ExceptionMessages.BadRequest);
                }

                dbContext.Clinic_Subspecialties.Add(new Clinic_Subspecialty
                {
                    DataId = subSpecialtyDto.Id,
                    SpecialtyId = subSpecialtyDto.SpecialtyId,
                    ConsultationLength = subSpecialtyDto.ConsultationLength,
                    UserId = userId
                });

                dbContext.SaveChanges();
            }
        }

        /// <summary>
        /// devuelve todas las subespecialidades de un usuario logueado
        /// </summary>
        [HttpGet]
        public List<SubspecialtyDto> GetAll()
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

                return dbContext.Clinic_Subspecialties
                    .Where(ssp => ssp.UserId == userId)
                    .Select(ssp => new SubspecialtyDto {
                        Id = ssp.Id,
                        Description = ssp.Data.Description,
                        ConsultationLength = ssp.ConsultationLength,
                        SpecialtyId = ssp.SpecialtyId,
                        SpecialtyDescription = ssp.Specialty.Data.Description
                }).ToList();
            }
        }

        /// <summary>
        /// Devuelve todas las subespecialidades que tiene una especialidad dada.
        /// </summary>
        [HttpPost]
        public List<SubspecialtyDto> GetAllOfSpecialty([FromBody] IdDto specialtyIdDto)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

                return dbContext.Clinic_Subspecialties
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

        /// <summary>
        /// Devuelve todas las subespecialidades con id de especialidad.
        /// </summary>
        [HttpPost]
        public List<SubspecialtyDto> GetAllOfSpecialtyNoUserID([FromBody] IdDto specialtyIdDto)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                return dbContext.Clinic_Subspecialties
                    .Where(ssp => ssp.SpecialtyId == specialtyIdDto.Id)
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

        /// <summary>
        /// Devuelev todas las subespecialidad de una especialidad dada para usar en SELECT2
        /// </summary>
        [HttpPost]
        public List<SelectOptionDto> GetAllOfSpecialtyForSelect([FromBody] IdDto specialtyIdDto)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

                return dbContext.Clinic_Subspecialties
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

        /// <summary>
        /// Elimna una subespecialidad
        /// </summary>
        [HttpPost]
        public void Remove([FromBody] IdDto subSpecialtyDto)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

                var subSpecialtyToDelete = dbContext.Clinic_Subspecialties.FirstOrDefault(ssp => ssp.Id == subSpecialtyDto.Id && ssp.UserId == userId);

                if (subSpecialtyToDelete == null)
                {
                    throw new BadRequestException(ExceptionMessages.BadRequest);
                }

                dbContext.Entry(subSpecialtyToDelete).State = EntityState.Deleted;
                dbContext.SaveChanges();
            }
        }

        /// <summary>
        /// Edita una subespecialidad
        /// </summary>
        [HttpPost]
        public void Edit([FromBody] EditSubspecialtyDto subSpecialtyDto)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

                var subSpecialtyToUpdate = dbContext.Clinic_Subspecialties.FirstOrDefault(s => s.Id == subSpecialtyDto.Id && s.UserId == userId);

                if (subSpecialtyToUpdate == null)
                {
                    throw new BadRequestException(ExceptionMessages.BadRequest);
                }

                subSpecialtyToUpdate.ConsultationLength = subSpecialtyDto.ConsultationLength;
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
