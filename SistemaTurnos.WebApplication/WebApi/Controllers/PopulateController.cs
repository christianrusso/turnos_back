using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SistemaTurnos.WebApplication.Database;
using SistemaTurnos.WebApplication.Database.Enums;
using SistemaTurnos.WebApplication.Database.Model;
using SistemaTurnos.WebApplication.WebApi.Authorization;
using SistemaTurnos.WebApplication.WebApi.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SistemaTurnos.WebApplication.WebApi.Controllers
{
    [Route("Api/[controller]/[action]")]
    [Produces("application/json")]
    [EnableCors("AnyOrigin")]
    public class PopulateController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public PopulateController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpPost]
        public void Populate()
        {
            CreateRoles();

            // Creo clinicas
            var clinic1 = CreateClinicUser("clinica1@asd.com", "clinica1@asd.com", "algun lugar 1", -12, 20);
            var clinic2 = CreateClinicUser("clinica2@asd.com", "clinica2@asd.com", "algun lugar 2", 22, 55);
            var clinic3 = CreateClinicUser("clinica3@asd.com", "clinica3@asd.com", "algun lugar 3", 22, 55);
            var clinic4 = CreateClinicUser("clinica4@asd.com", "clinica4@asd.com", "algun lugar 4", 22, 55);


            // Clinica 1
            // Creo empleados
            var employee1 = CreateEmployee("empleado1@asd.com", "empleado1@asd.com", clinic1);
            var employee2 = CreateEmployee("empleado2@asd.com", "empleado2@asd.com", clinic1);

            // Creo especialidades
            var specialty1 = CreateSpecialty("Kinesiologia", clinic1);
            var specialty2 = CreateSpecialty("Oftalmologia", clinic1);
            var specialty3 = CreateSpecialty("Farmacología", clinic1);
            var specialty4 = CreateSpecialty("Traumatología", clinic1);

            // Creo subespecialidades
            var subspecialty1 = CreateSubspecialty("Subespecialidad 1", specialty1, 10, clinic1);
            var subspecialty2 = CreateSubspecialty("Subespecialidad 2", specialty1, 20, clinic1);
            var subspecialty3 = CreateSubspecialty("Subespecialidad 3", specialty2, 30, clinic1);
            var subspecialty4 = CreateSubspecialty("Subespecialidad 4", specialty3, 40, clinic1);
            var subspecialty5 = CreateSubspecialty("Subespecialidad 5", specialty4, 50, clinic1);
            var subspecialty6 = CreateSubspecialty("Subespecialidad 6", specialty4, 60, clinic1);

            // Creo doctores 
            var doctor1 = CreateDoctor("Fernando", "Gomez", 30, specialty2, null, DoctorStateEnum.Active,
                new List<WorkingHours> {
                    new WorkingHours { DayNumber = DayOfWeek.Monday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(16, 0, 0) },
                    new WorkingHours { DayNumber = DayOfWeek.Tuesday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(16, 0, 0) },
                    new WorkingHours { DayNumber = DayOfWeek.Wednesday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(16, 0, 0) },
                    new WorkingHours { DayNumber = DayOfWeek.Thursday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(16, 0, 0) },
                    new WorkingHours { DayNumber = DayOfWeek.Friday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(16, 0, 0) },
                    new WorkingHours { DayNumber = DayOfWeek.Saturday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(16, 0, 0) },
                    new WorkingHours { DayNumber = DayOfWeek.Sunday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(16, 0, 0) },
                }, clinic1);

            var doctor2 = CreateDoctor("Christian", "Russo", 10, specialty1, subspecialty1, DoctorStateEnum.Active,
                new List<WorkingHours> {
                    new WorkingHours { DayNumber = DayOfWeek.Monday, Start = new TimeSpan(7, 0, 0), End = new TimeSpan(21, 0, 0) },
                    new WorkingHours { DayNumber = DayOfWeek.Tuesday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(16, 0, 0) },
                    new WorkingHours { DayNumber = DayOfWeek.Wednesday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(16, 0, 0) },
                    new WorkingHours { DayNumber = DayOfWeek.Thursday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(16, 0, 0) },
                    new WorkingHours { DayNumber = DayOfWeek.Friday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(16, 0, 0) },
                    new WorkingHours { DayNumber = DayOfWeek.Saturday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(16, 0, 0) },
                    new WorkingHours { DayNumber = DayOfWeek.Sunday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(16, 0, 0) },
                }, clinic1);


            var doctor3 = CreateDoctor("Sabrina", "Fillol", 10, specialty1, subspecialty1, DoctorStateEnum.Active,
                new List<WorkingHours> {
                    new WorkingHours { DayNumber = DayOfWeek.Monday, Start = new TimeSpan(7, 0, 0), End = new TimeSpan(21, 0, 0) },
                    new WorkingHours { DayNumber = DayOfWeek.Tuesday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(16, 0, 0) },
                    new WorkingHours { DayNumber = DayOfWeek.Wednesday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(16, 0, 0) },
                    new WorkingHours { DayNumber = DayOfWeek.Thursday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(16, 0, 0) },
                    new WorkingHours { DayNumber = DayOfWeek.Friday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(16, 0, 0) },
                    new WorkingHours { DayNumber = DayOfWeek.Saturday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(16, 0, 0) },
                    new WorkingHours { DayNumber = DayOfWeek.Sunday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(16, 0, 0) },
                }, clinic1);

            // Creo obras sociales
            var medicalInsurance1 = CreateMedicalInsurance("OSDE", clinic1);
            var medicalInsurance2 = CreateMedicalInsurance("Galeno", clinic1);
            var medicalInsurance3 = CreateMedicalInsurance("Swiss Medical", clinic1);

            // Creo planes de obras sociales
            var medicalPlan1 = CreateMedicalPlan("OSDE 210", medicalInsurance1, clinic1);
            var medicalPlan2 = CreateMedicalPlan("OSDE 310", medicalInsurance1, clinic1);
            var medicalPlan3 = CreateMedicalPlan("OSDE 410", medicalInsurance1, clinic1);
            var medicalPlan4 = CreateMedicalPlan("Galeno XL", medicalInsurance2, clinic1);
            var medicalPlan5 = CreateMedicalPlan("SM 10", medicalInsurance3, clinic1);
            var medicalPlan6 = CreateMedicalPlan("SM 20", medicalInsurance3, clinic1);

            // Creo clientes
            var client1 = CreateClientUser("cliente1@asd.com", "cliente1@asd.com");
            var client2 = CreateClientUser("cliente2@asd.com", "cliente2@asd.com");
            var client3 = CreateClientUser("cliente3@asd.com", "cliente3@asd.com");
            var client4 = CreateClientUser("cliente4@asd.com", "cliente4@asd.com");
            var client5 = CreateClientUser("cliente5@asd.com", "cliente5@asd.com");
            var client6 = CreateClientUser("cliente6@asd.com", "cliente6@asd.com");
            var client7 = CreateClientUser("cliente7@asd.com", "cliente7@asd.com");

            // Creo pacientes
            var patient1 = CreatePatient("Paciente", "1", "qwerty 1", "12345677", medicalPlan1, client1, clinic1);
            var patient2 = CreatePatient("Paciente", "2", "qwerty 2", "12345678", medicalPlan4, client2, clinic1);
            var patient3 = CreatePatient("Paciente", "3", "qwerty 3", "12345679", medicalPlan6, client3, clinic1);

            // Creo turnos
            var appointment1 = CreateAppointment(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 8, 30, 0), doctor1, patient1, AppointmentStateEnum.Reserved, null, clinic1);
            var appointment2 = CreateAppointment(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 10, 30, 0), doctor1, patient2, AppointmentStateEnum.Reserved, null, clinic1);
            var appointment3 = CreateAppointment(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 12, 0, 0), doctor1, patient1, AppointmentStateEnum.Reserved, null, clinic1);
            var appointment4 = CreateAppointment(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 15, 30, 0), doctor1, patient2, AppointmentStateEnum.Reserved, null, clinic1);
            var appointment5 = CreateAppointment(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 8, 40, 0), doctor2, patient2, AppointmentStateEnum.Reserved, null, clinic1);
            var appointment6 = CreateAppointment(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 8, 50, 0), doctor2, patient2, AppointmentStateEnum.Reserved, null, clinic1);
            var appointment7 = CreateAppointment(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 9, 20, 0), doctor2, patient1, AppointmentStateEnum.Reserved, null, clinic1);
            var appointment8 = CreateAppointment(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day+ 2, 9, 20, 0), doctor2, patient1, AppointmentStateEnum.Reserved, null, clinic1);
            var appointment9 = CreateAppointment(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day+1, 9, 20, 0), doctor2, patient2, AppointmentStateEnum.Reserved, null, clinic1);
            var appointment10 = CreateAppointment(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 9, 30, 0), doctor1, patient3, AppointmentStateEnum.Reserved, null, clinic1);
            var appointment11 = CreateAppointment(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 14, 20, 0), doctor2, patient1, AppointmentStateEnum.Reserved, null, clinic1);
            var appointment12 = CreateAppointment(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 15, 0, 0), doctor2, patient1, AppointmentStateEnum.Reserved, null, clinic1);
            var appointment13 = CreateAppointment(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day+3, 17, 0, 0), doctor1, patient2, AppointmentStateEnum.Reserved, null, clinic1);
            var appointment14 = CreateAppointment(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day+1, 9, 20, 0), doctor2, patient1, AppointmentStateEnum.Reserved, null, clinic1);
            var appointment15 = CreateAppointment(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day+7, 9, 20, 0), doctor2, patient2, AppointmentStateEnum.Reserved, null, clinic1);
            var appointment16 = CreateAppointment(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day+7, 9, 20, 0), doctor2, patient1, AppointmentStateEnum.Reserved, null, clinic1);
            
        }

        public void CreateRoles()
        {
            CreateRole(Roles.Administrator);
            CreateRole(Roles.Employee);
            CreateRole(Roles.Client);
        }

        private void CreateRole(string roleName)
        {
            bool exists = _roleManager.RoleExistsAsync(roleName).Result;

            if (!exists)
            {
                var role = new ApplicationRole
                {
                    Name = roleName
                };

                var result = _roleManager.CreateAsync(role).Result;

                if (!result.Succeeded)
                {
                    throw new ApplicationException(ExceptionMessages.InternalServerError);
                }
            }
        }

        private Clinic CreateClinicUser(string email, string password, string address, double latitude, double longitude)
        {
            Clinic clinic;

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

                result = _userManager.AddToRoleAsync(appUser, Roles.Administrator).Result;

                if (!result.Succeeded)
                {
                    throw new ApplicationException(ExceptionMessages.InternalServerError);
                }

                clinic = new Clinic
                {
                    Address = address,
                    Latitude = latitude,
                    Longitude = longitude,
                    UserId = appUser.Id
                };

                dbContext.Clinics.Add(clinic);
                dbContext.SaveChanges();
            }

            return clinic;
        }

        private Specialty CreateSpecialty(string description, Clinic clinic)
        {
            Specialty specialty;

            using (var dbContext = new ApplicationDbContext())
            {
                specialty = new Specialty
                {
                    Description = description,
                    UserId = clinic.UserId
                };

                dbContext.Specialties.Add(specialty);
                dbContext.SaveChanges();
            }

            return specialty;
        }

        private Subspecialty CreateSubspecialty(string description, Specialty specialty, uint consultationLength, Clinic clinic)
        {
            Subspecialty subspecialty;

            using (var dbContext = new ApplicationDbContext())
            {
                subspecialty = new Subspecialty
                {
                    Description = description,
                    SpecialtyId = specialty.Id,
                    ConsultationLength = consultationLength,
                    UserId = clinic.UserId
                };

                dbContext.Subspecialties.Add(subspecialty);
                dbContext.SaveChanges();
            }

            return subspecialty;
        }

        private Doctor CreateDoctor(string firstName, string lastName, uint consultationLength, Specialty specialty, Subspecialty subspecialty, DoctorStateEnum state, List<WorkingHours> workingHours, Clinic clinic)
        {
            Doctor doctor;

            using (var dbContext = new ApplicationDbContext())
            {
                doctor = new Doctor
                {
                    FirstName = firstName,
                    LastName = lastName,
                    PhoneNumber = string.Empty,
                    Email = string.Empty,
                    ConsultationLength = consultationLength,
                    SpecialtyId = specialty.Id,
                    SubspecialtyId = subspecialty?.Id,
                    State = state,
                    WorkingHours = workingHours,
                    UserId = clinic.UserId
                };

                dbContext.Doctors.Add(doctor);
                dbContext.SaveChanges();
            }

            return doctor;
        }

        private MedicalInsurance CreateMedicalInsurance(string description, Clinic clinic)
        {
            MedicalInsurance medicalInsurance;

            using (var dbContext = new ApplicationDbContext())
            {
                medicalInsurance = new MedicalInsurance
                {
                    Description = description,
                    UserId = clinic.UserId
                };

                dbContext.MedicalInsurances.Add(medicalInsurance);
                dbContext.SaveChanges();
            }

            return medicalInsurance;
        }

        private MedicalPlan CreateMedicalPlan(string description, MedicalInsurance medicalInsurance, Clinic clinic)
        {
            MedicalPlan medicalPlan;

            using (var dbContext = new ApplicationDbContext())
            {
                medicalPlan = new MedicalPlan
                {
                    Description = description,
                    MedicalInsuranceId = medicalInsurance.Id,
                    UserId = clinic.UserId
                };

                dbContext.MedicalPlans.Add(medicalPlan);
                dbContext.SaveChanges();
            }

            return medicalPlan;
        }

        private Client CreateClientUser(string email, string password)
        {
            Client client;

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

                client = new Client
                {
                    UserId = appUser.Id
                };

                dbContext.Clients.Add(client);
                dbContext.SaveChanges();
            }

            return client;
        }

        private Patient CreatePatient(string firstName, string lastName, string address, string dni, MedicalPlan medicalPlan, Client client, Clinic clinic)
        {
            Patient patient;

            using (var dbContext = new ApplicationDbContext())
            {
                patient = new Patient
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Address = address,
                    Dni = dni,
                    PhoneNumber = string.Empty,
                    MedicalPlanId = medicalPlan.Id,
                    ClientId = client.Id,
                    UserId = clinic.Id
                };

                dbContext.Patients.Add(patient);
                dbContext.SaveChanges();
            }

            return patient;
        }

        private Appointment CreateAppointment(DateTime dateTime, Doctor doctor, Patient patient, AppointmentStateEnum state, Rating rating, Clinic clinic)
        {
            Appointment appointment;

            using (var dbContext = new ApplicationDbContext())
            {
                appointment = new Appointment
                {
                    DateTime = dateTime,
                    DoctorId = doctor.Id,
                    PatientId = patient.Id,
                    State = state,
                    RatingId = rating?.Id ?? 0,
                    UserId = clinic.UserId
                };

                dbContext.Appointments.Add(appointment);
                dbContext.SaveChanges();
            }

            return appointment;
        }

        private Employee CreateEmployee(string email, string password, Clinic clinic)
        {
            Employee employee;

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

                result = _userManager.AddToRoleAsync(appUser, Roles.Employee).Result;

                if (!result.Succeeded)
                {
                    throw new ApplicationException(ExceptionMessages.InternalServerError);
                }

                employee = new Employee
                {
                    UserId = appUser.Id,
                    OwnerUserId = clinic.UserId
                };

                dbContext.Employees.Add(employee);
                dbContext.SaveChanges();
            }

            return employee;
        }
    }
}
