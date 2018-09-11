using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaTurnos.WebApplication.Database;
using SistemaTurnos.WebApplication.Database.Enums;
using SistemaTurnos.WebApplication.Database.HairdressingModel;
using SistemaTurnos.WebApplication.Database.Model;
using SistemaTurnos.WebApplication.WebApi.Authorization;
using SistemaTurnos.WebApplication.WebApi.Dto.HairdressingPatient;
using SistemaTurnos.WebApplication.WebApi.Exceptions;
using SistemaTurnos.WebApplication.WebApi.Services;

namespace SistemaTurnos.WebApplication.WebApi.Controllers
{
    [Route("Api/Hairdressing/[controller]/[action]")]
    [Produces("application/json")]
    [EnableCors("AnyOrigin")]
    [Authorize(Roles = Roles.AdministratorAndEmployee)]
    public class HairdressingPatientController : Controller
    {
        private BusinessPlaceService _service;

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public HairdressingPatientController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _service = new BusinessPlaceService(this.HttpContext);
        }

        [HttpPost]
        public void Add([FromBody] AddHairdressingPatientDto patientDto)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _service.GetUserId();

                var client = dbContext.Hairdressing_Clients.FirstOrDefault(c => c.Id == patientDto.ClientId);
                
                if (client == null)
                {
                    if (string.IsNullOrWhiteSpace(patientDto.Email))
                    {
                        throw new ApplicationException(ExceptionMessages.BadRequest);
                    }

                    client = CreateClient(patientDto.Email, patientDto.Dni);
                }

                dbContext.Hairdressing_Patients.Add(new Hairdressing_Patient
                {
                    FirstName = patientDto.FirstName,
                    LastName = patientDto.LastName,
                    Address = patientDto.Address,
                    PhoneNumber = patientDto.PhoneNumber,
                    Dni = patientDto.Dni,
                    UserId = userId,
                    ClientId = patientDto.ClientId
                });

                dbContext.SaveChanges();
            }
        }

        [HttpPost]
        public void AddForNonClient([FromBody] AddHairdressingPatientForNonClientDto patientDto)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _service.GetUserId();

                if (!_roleManager.RoleExistsAsync(Roles.Client).Result)
                {
                    throw new ApplicationException(ExceptionMessages.RolesHaveNotBeenCreated);
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

                var client = new Hairdressing_Client
                {
                    UserId = appUser.Id
                };

                dbContext.Hairdressing_Clients.Add(client);
                dbContext.SaveChanges();

                var patient = new Hairdressing_Patient
                {
                    FirstName = patientDto.FirstName,
                    LastName = patientDto.LastName,
                    Address = patientDto.Address,
                    PhoneNumber = patientDto.PhoneNumber,
                    Dni = patientDto.Dni,
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
                var userId = _service.GetUserId();

                return dbContext.Hairdressing_Patients
                    .Where(p => p.UserId == userId)
                    .Select(s => new HairdressingPatientDto
                    {
                        Id = s.Id,
                        FirstName = s.FirstName,
                        LastName = s.LastName,
                        Address = s.Address,
                        PhoneNumber = s.PhoneNumber,
                        Email = s.Client.User.Email,
                        Dni = s.Dni,
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
                var userId = _service.GetUserId();

                var patientToDelete = dbContext.Hairdressing_Patients.FirstOrDefault(p => p.Id == patientDto.Id && p.UserId == userId);

                if (patientToDelete == null)
                {
                    throw new BadRequestException(ExceptionMessages.BadRequest);
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
                var userId = _service.GetUserId();

                var patientToUpdate = dbContext.Clinic_Patients.FirstOrDefault(p => p.Id == patientDto.Id && p.UserId == userId);

                if (patientToUpdate == null)
                {
                    throw new BadRequestException(ExceptionMessages.BadRequest);
                }

                patientToUpdate.FirstName = patientDto.FirstName;
                patientToUpdate.LastName = patientDto.LastName;
                patientToUpdate.Address = patientDto.Address;
                patientToUpdate.PhoneNumber = patientDto.PhoneNumber;
                patientToUpdate.Dni = patientDto.Dni;
                dbContext.SaveChanges();
            }
        }

        [HttpPost]
        public List<HairdressingPatientDto> GetByFilter([FromBody] FilterHairdressingPatientDto filter)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _service.GetUserId();

                return dbContext.Hairdressing_Patients
                    .Where(p => p.UserId == userId)
                    .Select(s => new HairdressingPatientDto()
                    {
                        Id = s.Id,
                        FirstName = s.FirstName,
                        LastName = s.LastName,
                        Address = s.Address,
                        PhoneNumber = s.PhoneNumber,
                        Email = s.Client.User.Email,
                        Dni = s.Dni,
                        UserId = s.UserId,
                        ClientId = s.ClientId,
                        ReservedAppointments = s.Appointments.Count(),
                        ConcretedAppointments = s.Appointments.Count(a => a.State == AppointmentStateEnum.Completed)
                    }).ToList();
            }
        }

        private Hairdressing_Client CreateClient(string email, string password)
        {
            if (!_roleManager.RoleExistsAsync(Roles.Client).Result)
            {
                throw new ApplicationException(ExceptionMessages.RolesHaveNotBeenCreated);
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

            using (var dbContext = new ApplicationDbContext())
            {
                var appUser = _userManager.Users.SingleOrDefault(au => au.Email == email);

                result = _userManager.AddToRoleAsync(appUser, Roles.Client).Result;

                if (!result.Succeeded)
                {
                    throw new ApplicationException(ExceptionMessages.InternalServerError);
                }

                var client = new Hairdressing_Client
                {
                    UserId = appUser.Id
                };

                dbContext.Hairdressing_Clients.Add(client);
                dbContext.SaveChanges();

                return dbContext.Hairdressing_Clients.FirstOrDefault(c => c.UserId == appUser.Id);
            }
        }
    }
}
