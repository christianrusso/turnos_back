using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaTurnos.Commons.Authorization;
using SistemaTurnos.Commons.Exceptions;
using SistemaTurnos.Database;
using SistemaTurnos.Database.ClinicModel;
using SistemaTurnos.Database.Enums;
using SistemaTurnos.Database.HairdressingModel;
using SistemaTurnos.Database.Model;
using SistemaTurnos.WebApplication.WebApi.Dto;
using SistemaTurnos.WebApplication.WebApi.Dto.HairdressingPatient;
using SistemaTurnos.WebApplication.WebApi.Dto.Record;
using SistemaTurnos.WebApplication.WebApi.Services;

namespace SistemaTurnos.WebApplication.WebApi.Controllers
{
    [Route("Api/Hairdressing/[controller]/[action]")]
    [Produces("application/json")]
    [EnableCors("AnyOrigin")]
    [Authorize(Roles = Roles.AdministratorAndEmployee)]
    public class HairdressingPatientController : Controller
    {
        public BusinessPlaceService _service;

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public HairdressingPatientController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _service = new BusinessPlaceService();
        }

        [HttpPost]
        public ActionResult Add([FromBody] AddHairdressingPatientDto patientDto)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _service.GetUserId(HttpContext);

                var client = dbContext.Clients.FirstOrDefault(c => c.Id == patientDto.ClientId);
                
                if (client == null)
                {
                    if (string.IsNullOrWhiteSpace(patientDto.Email))
                    {
                        throw new BadRequestException();
                    }

                    client = CreateClient(patientDto.Email, patientDto.Dni, patientDto);
                }

                dbContext.Hairdressing_Patients.Add(new Hairdressing_Patient
                {
                    
                    UserId = userId,
                    ClientId = patientDto.ClientId
                });

                var id = dbContext.SaveChanges();

                return Ok(id);
            }
        }

        [HttpPost]
        public void AddForNonClient([FromBody] AddHairdressingPatientForNonClientDto patientDto)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _service.GetUserId(this.HttpContext);

                if (!_roleManager.RoleExistsAsync(Roles.Client).Result)
                {
                    throw new ApplicationException(ExceptionMessages.InternalServerError);
                }

                var user = new ApplicationUser
                {
                    UserName = patientDto.Email,
                    Email = patientDto.Email
                };

                var result = _userManager.CreateAsync(user, patientDto.Password).Result;

                if (!result.Succeeded)
                {
                    throw new ApplicationException(ExceptionMessages.UsernameAlreadyExists);
                }

                var appUser = _userManager.Users.SingleOrDefault(au => au.Email == patientDto.Email);

                result = _userManager.AddToRoleAsync(appUser, Roles.Client).Result;

                if (!result.Succeeded)
                {
                    throw new ApplicationException(ExceptionMessages.InternalServerError);
                }

                var client = new SystemClient
                {
                    UserId = appUser.Id,
                    FirstName = patientDto.FirstName,
                    LastName = patientDto.LastName,
                    Address = patientDto.Address,
                    PhoneNumber = patientDto.PhoneNumber,
                    Dni = patientDto.Dni,
                };

                dbContext.Clients.Add(client);
                dbContext.SaveChanges();

                var patient = new Hairdressing_Patient
                {
                    
                    UserId = userId,
                    ClientId = client.Id
                };

                dbContext.Hairdressing_Patients.Add(patient);
                dbContext.SaveChanges();
            }
        }

        [HttpGet]
        public List<HairdressingPatientDto> GetAll()
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _service.GetUserId(this.HttpContext);

                return dbContext.Hairdressing_Patients
                    .Where(p => p.UserId == userId)
                    .Select(s => new HairdressingPatientDto
                    {
                        Id = s.Id,
                        FirstName = s.Client.FirstName,
                        LastName = s.Client.LastName,
                        Address = s.Client.Address,
                        PhoneNumber = s.Client.PhoneNumber,
                        Email = s.Client.User.Email,
                        Dni = s.Client.Dni,
                        UserId = s.UserId,
                        ClientId = s.ClientId,
                        ReservedAppointments = s.Appointments.Count(),
                        ConcretedAppointments = s.Appointments.Count(a => a.State == AppointmentStateEnum.Completed)
                    }).ToList();
            }
        }

        [HttpPost]
        public void Remove([FromBody] RemoveHairdressingPatientDto patientDto)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _service.GetUserId(HttpContext);

                var patientToDelete = dbContext.Hairdressing_Patients.FirstOrDefault(p => p.Id == patientDto.Id && p.UserId == userId);

                if (patientToDelete == null)
                {
                    throw new BadRequestException();
                }

                dbContext.Entry(patientToDelete).State = EntityState.Deleted;
                dbContext.SaveChanges();
            }
        }

        [HttpPost]
        public void Edit([FromBody] EditHairdressingPatientDto patientDto)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _service.GetUserId(HttpContext);

                var patientToUpdate = dbContext.Hairdressing_Patients.FirstOrDefault(p => p.Id == patientDto.Id && p.UserId == userId);

                if (patientToUpdate == null)
                {
                    throw new BadRequestException();
                }

                patientToUpdate.Client.FirstName = patientDto.FirstName;
                patientToUpdate.Client.LastName = patientDto.LastName;
                patientToUpdate.Client.Address = patientDto.Address;
                patientToUpdate.Client.PhoneNumber = patientDto.PhoneNumber;
                patientToUpdate.Client.Dni = patientDto.Dni;
                dbContext.SaveChanges();
            }
        }

        [HttpPost]
        public List<HairdressingPatientDto> GetByFilter([FromBody] FilterHairdressingPatientDto filter)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _service.GetUserId(HttpContext);

                return dbContext.Hairdressing_Patients
                    .Where(p => p.UserId == userId)
                    .Where(p => string.IsNullOrWhiteSpace(filter.Text) || p.Client.FullName.ToLower().Contains(filter.Text.ToLower()) || p.Client.User.Email.ToLower().Contains(filter.Text.ToLower()))
                    .Select(s => new HairdressingPatientDto()
                    {
                        Id = s.Id,
                        FirstName = s.Client.FirstName,
                        LastName = s.Client.LastName,
                        Address = s.Client.Address,
                        PhoneNumber = s.Client.PhoneNumber,
                        Email = s.Client.User.Email,
                        Dni = s.Client.Dni,
                        UserId = s.UserId,
                        ClientId = s.ClientId,
                        ReservedAppointments = s.Appointments.Count(),
                        ConcretedAppointments = s.Appointments.Count(a => a.State == AppointmentStateEnum.Completed)
                    }).ToList();
            }
        }

        [HttpPost]
        public void AddRecord([FromBody] RecordDto dto)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _service.GetUserId(HttpContext);

                var patient = dbContext.Hairdressing_Patients.FirstOrDefault(p => p.UserId == userId && p.Id == dto.Id);

                if (patient == null)
                {
                    throw new BadRequestException();
                }

                patient.Records.Add(new Hairdressing_Record
                {
                    DateTime = dto.DateTime,
                    Description = dto.Description,
                    PatientId = dto.Id
                });

                dbContext.SaveChanges();
            }
        }

        [HttpPost]
        public void EditMedicalRecord([FromBody] RecordDto dto)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _service.GetUserId(HttpContext);

                var record = dbContext.Hairdressing_Records.FirstOrDefault(r => r.Id == dto.Id && r.Patient.UserId == userId);

                if (record == null)
                {
                    throw new BadRequestException();
                }

                record.DateTime = dto.DateTime;
                record.Description = dto.Description;
                dbContext.SaveChanges();
            }
        }

        [HttpPost]
        public void RemoveMedicalRecord([FromBody] IdDto dto)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _service.GetUserId(HttpContext);

                var record = dbContext.Hairdressing_Records.FirstOrDefault(r => r.Id == dto.Id && r.Patient.UserId == userId);

                if (record == null)
                {
                    throw new BadRequestException();
                }

                dbContext.Entry(record).State = EntityState.Deleted;
                dbContext.SaveChanges();
            }
        }

        [HttpPost]
        public List<RecordDto> GetMedicalRecords([FromBody] IdDto dto)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _service.GetUserId(HttpContext);

                var patient = dbContext.Hairdressing_Patients.FirstOrDefault(p => p.Id == dto.Id);

                if (patient == null)
                {
                    throw new BadRequestException();
                }

                return patient.Records.Select(r => new RecordDto
                {
                    Id = r.Id,
                    Description = r.Description,
                    DateTime = r.DateTime
                })
                .OrderByDescending(r => r.DateTime)
                .ToList();
            }
        }

        private SystemClient CreateClient(string email, string password, AddHairdressingPatientDto patientDto)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                if (dbContext.Clients.Any(c => c.Dni == patientDto.Dni))
                {
                    throw new ApplicationException(ExceptionMessages.UsernameAlreadyExists);
                }

                if (!_roleManager.RoleExistsAsync(Roles.Client).Result)
                {
                    throw new ApplicationException(ExceptionMessages.InternalServerError);
                }

                var user = new ApplicationUser
                {
                    UserName = email,
                    Email = email
                };

                var result = _userManager.CreateAsync(user, password).Result;

                if (!result.Succeeded)
                {
                    throw new ApplicationException(ExceptionMessages.UsernameAlreadyExists);
                }
            
                var appUser = _userManager.Users.SingleOrDefault(au => au.Email == email);

                result = _userManager.AddToRoleAsync(appUser, Roles.Client).Result;

                if (!result.Succeeded)
                {
                    throw new ApplicationException(ExceptionMessages.InternalServerError);
                }

                var client = new SystemClient
                {
                    UserId = appUser.Id,
                    FirstName = patientDto.FirstName,
                    LastName = patientDto.LastName,
                    Address = patientDto.Address,
                    PhoneNumber = patientDto.PhoneNumber,
                    Dni = patientDto.Dni,
                };

                dbContext.Clients.Add(client);
                dbContext.SaveChanges();

                return dbContext.Clients.FirstOrDefault(c => c.UserId == appUser.Id);
            }
        }
    }
}
