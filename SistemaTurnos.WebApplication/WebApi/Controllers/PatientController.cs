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
using SistemaTurnos.Database.Model;
using SistemaTurnos.WebApplication.Database.Model;
using SistemaTurnos.WebApplication.WebApi.Dto;
using SistemaTurnos.WebApplication.WebApi.Dto.Patient;
using SistemaTurnos.WebApplication.WebApi.Dto.Record;
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
            _service = new BusinessPlaceService();
        }

        /// <summary>
        /// Agrega un paciente que previamente es cliente.
        /// </summary>
        [HttpPost]
        public void Add([FromBody] AddPatientDto patientDto)
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

                var medicalPlan = dbContext.Clinic_MedicalPlans.FirstOrDefault(mp => mp.Id == patientDto.MedicalPlanId);

                dbContext.Clinic_Patients.Add(new Clinic_Patient
                {
                    UserId = userId,
                    ClientId = patientDto.ClientId,
                    MedicalPlan = medicalPlan
                });

                dbContext.SaveChanges();
            }
        }

        /// <summary>
        /// Agrega un paciente que no es cliente
        /// </summary>
        [HttpPost]
        public void AddForNonClient([FromBody] AddPatientForNonClientDto patientDto)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _service.GetUserId(HttpContext);

                var medicalPlan = dbContext.Clinic_MedicalPlans.FirstOrDefault(mp => mp.Id == patientDto.MedicalPlanId);

                if (medicalPlan == null)
                {
                    throw new BadRequestException();
                }

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

                var patient = new Clinic_Patient
                {
                    
                    UserId = userId,
                    ClientId = client.Id,
                    MedicalPlanId = patientDto.MedicalPlanId
                };

                dbContext.Clinic_Patients.Add(patient);
                dbContext.SaveChanges();
            }
        }

        /// <summary>
        /// Obtiene todos los pacientes
        /// </summary>
        [HttpGet]
        public List<PatientDto> GetAll()
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _service.GetUserId(HttpContext);

                return dbContext.Clinic_Patients
                    .Where(p => p.UserId == userId)
                    .Select(s => new PatientDto {
                        Id = s.Id,
                        FirstName = s.Client.FirstName,
                        LastName = s.Client.LastName,
                        MedicalPlan = s.MedicalPlan.Data.Description,
                        MedicalPlanId = s.MedicalPlan.Id,
                        MedicalInsurance = s.MedicalPlan.MedicalInsurance.Data.Description,
                        MedicalInsuranceId = s.MedicalPlan.MedicalInsurance.Id,
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

        /// <summary>
        /// Remueve un paciente
        /// </summary>
        [HttpPost]
        public void Remove([FromBody] RemovePatientDto patientDto)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _service.GetUserId(HttpContext);

                var patientToDelete = dbContext.Clinic_Patients.FirstOrDefault(p => p.Id == patientDto.Id && p.UserId == userId);

                if (patientToDelete == null)
                {
                    throw new BadRequestException();
                }

                dbContext.Entry(patientToDelete).State = EntityState.Deleted;
                dbContext.SaveChanges();
            }
        }

        /// <summary>
        /// Edita un paciente
        /// </summary>
        [HttpPost]
        public void Edit([FromBody] EditPatientDto patientDto)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _service.GetUserId(HttpContext);

                var patientToUpdate = dbContext.Clinic_Patients.FirstOrDefault(p => p.Id == patientDto.Id && p.UserId == userId);

                if (patientToUpdate == null)
                {
                    throw new BadRequestException();
                }

                var medicalPlan = dbContext.Clinic_MedicalPlans.FirstOrDefault(mp => mp.Id == patientDto.MedicalPlanId);

                if (medicalPlan == null)
                {
                    throw new BadRequestException();
                }

                patientToUpdate.Client.FirstName = patientDto.FirstName;
                patientToUpdate.Client.LastName = patientDto.LastName;
                patientToUpdate.Client.Address = patientDto.Address;
                patientToUpdate.Client.PhoneNumber = patientDto.PhoneNumber;
                patientToUpdate.Client.Dni = patientDto.Dni;
                patientToUpdate.MedicalPlanId = patientDto.MedicalPlanId;
                dbContext.SaveChanges();
            }
        }

        /// <summary>
        /// OBtiene los pacientes con filtros
        /// </summary>
        [HttpPost]
        public List<PatientDto> GetByFilter([FromBody] FilterPatientDto filter)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _service.GetUserId(HttpContext);

                return dbContext.Clinic_Patients
                    .Where(p => p.UserId == userId)
                    .Where(p => string.IsNullOrWhiteSpace(filter.Text) || p.Client.FullName.ToLower().Contains(filter.Text.ToLower()) || p.Client.User.Email.ToLower().Contains(filter.Text.ToLower()))
                    .Where(p => filter.MedicalInsuranceId == null | p.MedicalPlan.MedicalInsuranceId == filter.MedicalInsuranceId)
                    .Select(s => new PatientDto
                    {
                        Id = s.Id,
                        FirstName = s.Client.FirstName,
                        LastName = s.Client.LastName,
                        MedicalPlan = s.MedicalPlan.Data.Description,
                        MedicalPlanId = s.MedicalPlanId,
                        MedicalInsurance = s.MedicalPlan.MedicalInsurance.Data.Description,
                        MedicalInsuranceId = s.MedicalPlan.MedicalInsurance.Id,
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
        public void AddMedicalRecord([FromBody] RecordDto dto)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _service.GetUserId(HttpContext);

                var patient = dbContext.Clinic_Patients.FirstOrDefault(p => p.UserId == userId && p.Id == dto.Id);

                if (patient == null)
                {
                    throw new BadRequestException();
                }

                patient.MedicalRecords.Add(new Clinic_Record {
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

                var record = dbContext.Clinic_Records.FirstOrDefault(r => r.Id == dto.Id && r.Patient.UserId == userId);

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

                var record = dbContext.Clinic_Records.FirstOrDefault(r => r.Id == dto.Id && r.Patient.UserId == userId);

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

                var patient = dbContext.Clinic_Patients.FirstOrDefault(p => p.Id == dto.Id);

                if (patient == null)
                {
                    throw new BadRequestException();
                }

                return patient.MedicalRecords.Select(r => new RecordDto
                {
                    Id = r.Id,
                    Description = r.Description,
                    DateTime = r.DateTime
                })
                .OrderByDescending(r => r.DateTime)
                .ToList();
            }
        }

        [HttpPost]

        private SystemClient CreateClient(string email, string password, AddPatientDto patientDto)
        {
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

            using (var dbContext = new ApplicationDbContext())
            {
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
