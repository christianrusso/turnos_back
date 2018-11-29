using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaTurnos.Commons.Exceptions;
using SistemaTurnos.Database;
using SistemaTurnos.Database.ClinicModel;
using SistemaTurnos.WebApplication.WebApi.Dto;
using SistemaTurnos.WebApplication.WebApi.Dto.Specialty;
using SistemaTurnos.WebApplication.WebApi.Dto.Subspecialty;

namespace SistemaTurnos.WebApplication.WebApi.Controllers
{
    [Route("Api/[controller]/[action]")]
    [Produces("application/json")]
    [EnableCors("AnyOrigin")]
    public class SpecialtyController : Controller
    {
        /// <summary>
        ///Agrega una especialidad a una clinica logueada (el parametros el id de la especialdiad en data)
        /// </summary>
        [HttpPost]
        public void Add([FromBody] IdDto specialtyDto)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

                dbContext.Clinic_Specialties.Add(new Clinic_Specialty
                {
                    DataId = specialtyDto.Id,
                    Subspecialties = new List<Clinic_Subspecialty>(),
                    UserId = userId
                });

                dbContext.SaveChanges();
            }
        }

        /// <summary>
        ///Obtiene todas las especialidaes de una clinica logeuada
        /// </summary>
        [HttpGet]
        public List<SpecialtyDto> GetAll()
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

                return dbContext.Clinic_Specialties
                    .Where(s => s.UserId == userId)
                    .Select(s => new SpecialtyDto
                    {
                        Id = s.Id,
                        Description = s.Data.Description,
                        Doctors = s.Doctors.Count,
                        Subspecialties = s.Subspecialties
                            .Select(ssp => new SubspecialtyDto
                            {
                                Id = ssp.Id,
                                Description = ssp.Data.Description,
                                ConsultationLength = ssp.ConsultationLength
                            })
                            .ToList()
                    })
                    .ToList();
            }
        }

        /// <summary>
        ///obtiene todas las especiadliades de uan clinica dada.
        /// </summary>
        [HttpPost]
        public List<SpecialtyDto> GetAllByClinic([FromBody] IdDto idDto)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var clinic = dbContext.Clinics.FirstOrDefault(c => c.Id == idDto.Id);

                if (clinic == null)
                {
                    throw new BadRequestException();
                }

                return dbContext.Clinic_Specialties
                    .Where(s => s.UserId == clinic.UserId)
                    .Select(s => new SpecialtyDto
                    {
                        Id = s.Id,
                        Description = s.Data.Description,
                        Doctors = s.Doctors.Count,
                        Subspecialties = s.Subspecialties
                            .Select(ssp => new SubspecialtyDto
                            {
                                Id = ssp.Id,
                                Description = ssp.Data.Description,
                                ConsultationLength = ssp.ConsultationLength
                            })
                            .ToList()
                    })
                    .ToList();
            }
        }

        /// <summary>
        /// Obtiene todas las especialidaes de una clinica logeuada para usar en campos SELECT2
        /// </summary>
        [HttpGet]
        public List<SelectOptionDto> GetAllForSelect()
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

                return dbContext.Clinic_Specialties
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

        /// <summary>
        ///Obtiene todas las especialidaes de una clinica logeuada por la letra inicial
        /// </summary>
        [HttpPost]
        public List<SpecialtyDto> GetByLetter([FromBody] GetSubspecialtyByLetterDto filter)
        {
            var firstLetterMinus = char.ToLower(filter.Letter);
            var firstLetterMayus = char.ToUpper(filter.Letter);

            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

                var subspecialties = dbContext.Clinic_Subspecialties.Where(ssp => ssp.UserId == userId);

                return dbContext.Clinic_Specialties
                    .Where(s => s.UserId == userId)
                    .Where(ssp => filter.Letter == '*' || ssp.Data.Description.FirstOrDefault() == firstLetterMinus || ssp.Data.Description.FirstOrDefault() == firstLetterMayus)
                    .OrderBy(s => s.Data.Description)
                    .Select(s => new SpecialtyDto
                    {
                        Id = s.Id,
                        Description = s.Data.Description,
                        Doctors = s.Doctors.Count,
                        Subspecialties = subspecialties.Where(ssp => ssp.SpecialtyId == s.Id).Select(ssp => new SubspecialtyDto
                            {
                                Id = ssp.Id,
                                Description = ssp.Data.Description,
                                ConsultationLength = ssp.ConsultationLength
                            }).ToList()
                    }).ToList();
            }
        }

        /// <summary>
        ///Obtiene todas las especialidaes de una clinica logeuada con filtros
        /// </summary>
        [HttpPost]
        public List<SpecialtyDto> GetByFilter([FromBody] FilterSpecialtyDto filter)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

                var subspecialties = dbContext.Clinic_Subspecialties.Where(ssp => ssp.UserId == userId);

                return dbContext.Clinic_Specialties
                    .Where(s => s.UserId == userId)
                    .Where(ssp => ssp.Data.Description.Contains(filter.Description))
                    .Select(s => new SpecialtyDto
                    {
                        Id = s.Id,
                        Description = s.Data.Description,
                        Doctors = s.Doctors.Count,
                        Subspecialties = subspecialties.Where(ssp => ssp.SpecialtyId == s.Id).Select(ssp => new SubspecialtyDto
                        {
                            Id = ssp.Id,
                            Description = ssp.Data.Description,
                            ConsultationLength = ssp.ConsultationLength
                        }).ToList()
                    }).ToList();
            }
        }

        /// <summary>
        ///Elimina una especialidad de la clinica logueada
        /// </summary>
        [HttpPost]
        public void Remove([FromBody] IdDto specialtyDto)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();
                var specialtyToDelete = dbContext.Clinic_Specialties.FirstOrDefault(s => s.Id == specialtyDto.Id && s.UserId == userId);

                if (specialtyToDelete == null)
                {
                    throw new BadRequestException();
                }

                specialtyToDelete.Subspecialties.ForEach(ssp => dbContext.Entry(ssp).State = EntityState.Deleted);
                dbContext.Entry(specialtyToDelete).State = EntityState.Deleted;
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
