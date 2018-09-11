using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaTurnos.WebApplication.Database;
using SistemaTurnos.WebApplication.Database.ClinicModel;
using SistemaTurnos.WebApplication.Database.Enums;
using SistemaTurnos.WebApplication.Database.Model;
using SistemaTurnos.WebApplication.WebApi.Authorization;
using SistemaTurnos.WebApplication.WebApi.Dto.Patient;
using SistemaTurnos.WebApplication.WebApi.Exceptions;
using SistemaTurnos.WebApplication.WebApi.Services;

namespace SistemaTurnos.WebApplication.WebApi.Controllers
{
    [Route("Api/[controller]/[action]")]
    [Produces("application/json")]
    [EnableCors("AnyOrigin")]
    [Authorize(Roles = Roles.AdministratorAndEmployee)]
    public class PatientController : Controller
    {
        private BusinessPlaceService _service;

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public PatientController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _service = new BusinessPlaceService(this.HttpContext);
        }

        [HttpPost]
        public void Add([FromBody] AddPatientDto patientDto)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _service.GetUserId();

                var client = dbContext.Clinic_Clients.FirstOrDefault(c => c.Id == patientDto.ClientId);
                
                if (client == null)
                {
                    if (string.IsNullOrWhiteSpace(patientDto.Email))
                    {
                        throw new ApplicationException(ExceptionMessages.BadRequest);
                    }

                    client = CreateClient(patientDto.Email, patientDto.Dni);
                }

                var medicalPlan = dbContext.Clinic_MedicalPlans.FirstOrDefault(mp => mp.Id == patientDto.MedicalPlanId);

                dbContext.Clinic_Patients.Add(new Clinic_Patient
                {
                    FirstName = patientDto.FirstName,
                    LastName = patientDto.LastName,
                    Address = patientDto.Address,
                    PhoneNumber = patientDto.PhoneNumber,
                    Dni = patientDto.Dni,
                    UserId = userId,
                    ClientId = patientDto.ClientId,
                    MedicalPlan = medicalPlan
                });

                dbContext.SaveChanges();
            }
        }

        [HttpPost]
        public void AddForNonClient([FromBody] AddPatientForNonClientDto patientDto)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _service.GetUserId();

                var medicalPlan = dbContext.Clinic_MedicalPlans.FirstOrDefault(mp => mp.Id == patientDto.MedicalPlanId);

                if (medicalPlan == null)
                {
                    throw new ApplicationException(ExceptionMessages.BadRequest);
                }

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

                var client = new Clinic_Client
                {
                    UserId = appUser.Id
                };

                dbContext.Clinic_Clients.Add(client);
                dbContext.SaveChanges();

                var patient = new Clinic_Patient
                {
                    FirstName = patientDto.FirstName,
                    LastName = patientDto.LastName,
                    Address = patientDto.Address,
                    PhoneNumber = patientDto.PhoneNumber,
                    Dni = patientDto.Dni,
                    UserId = userId,
                    ClientId = client.Id,
                    MedicalPlanId = patientDto.MedicalPlanId
                };

                dbContext.Clinic_Patients.Add(patient);
                dbContext.SaveChanges();
            }
        }

        [HttpGet]
        public List<PatientDto> GetAll()
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _service.GetUserId();

                return dbContext.Clinic_Patients
                    .Where(p => p.UserId == userId)
                    .Select(s => new PatientDto {
                        Id = s.Id,
                        FirstName = s.FirstName,
                        LastName = s.LastName,
                        MedicalPlan = s.MedicalPlan.Data.Description,
                        MedicalPlanId = s.MedicalPlan.Id,
                        MedicalInsurance = s.MedicalPlan.MedicalInsurance.Data.Description,
                        MedicalInsuranceId = s.MedicalPlan.MedicalInsurance.Id,
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
        public void Remove([FromBody] RemovePatientDto patientDto)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _service.GetUserId();

                var patientToDelete = dbContext.Clinic_Patients.FirstOrDefault(p => p.Id == patientDto.Id && p.UserId == userId);

                if (patientToDelete == null)
                {
                    throw new BadRequestException(ExceptionMessages.BadRequest);
                }

                dbContext.Entry(patientToDelete).State = EntityState.Deleted;
                dbContext.SaveChanges();
            }
        }

        [HttpPost]
        public void Edit([FromBody] EditPatientDto patientDto)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _service.GetUserId();

                var patientToUpdate = dbContext.Clinic_Patients.FirstOrDefault(p => p.Id == patientDto.Id && p.UserId == userId);

                if (patientToUpdate == null)
                {
                    throw new BadRequestException(ExceptionMessages.BadRequest);
                }

                var medicalPlan = dbContext.Clinic_MedicalPlans.FirstOrDefault(mp => mp.Id == patientDto.MedicalPlanId);

                if (medicalPlan == null)
                {
                    throw new ApplicationException(ExceptionMessages.BadRequest);
                }

                patientToUpdate.FirstName = patientDto.FirstName;
                patientToUpdate.LastName = patientDto.LastName;
                patientToUpdate.Address = patientDto.Address;
                patientToUpdate.PhoneNumber = patientDto.PhoneNumber;
                patientToUpdate.Dni = patientDto.Dni;
                patientToUpdate.MedicalPlanId = patientDto.MedicalPlanId;
                dbContext.SaveChanges();
            }
        }

        [HttpPost]
        public List<PatientDto> GetByFilter([FromBody] FilterPatientDto filter)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _service.GetUserId();

                return dbContext.Clinic_Patients
                    .Where(p => p.UserId == userId)
                    .Where(p => filter.MedicalInsuranceId == null | p.MedicalPlan.MedicalInsuranceId == filter.MedicalInsuranceId)
                    .Select(s => new PatientDto
                    {
                        Id = s.Id,
                        FirstName = s.FirstName,
                        LastName = s.LastName,
                        MedicalPlan = s.MedicalPlan.Data.Description,
                        MedicalPlanId = s.MedicalPlanId,
                        MedicalInsurance = s.MedicalPlan.MedicalInsurance.Data.Description,
                        MedicalInsuranceId = s.MedicalPlan.MedicalInsurance.Id,
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

        private Clinic_Client CreateClient(string email, string password)
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

                var client = new Clinic_Client
                {
                    UserId = appUser.Id
                };

                dbContext.Clinic_Clients.Add(client);
                dbContext.SaveChanges();

                return dbContext.Clinic_Clients.FirstOrDefault(c => c.UserId == appUser.Id);
            }
        }
    }
}
