using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SistemaTurnos.WebApplication.Database;
using SistemaTurnos.WebApplication.Database.ClinicModel;
using SistemaTurnos.WebApplication.Database.Enums;
using SistemaTurnos.WebApplication.Database.Model;
using SistemaTurnos.WebApplication.Database.ModelData;
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
            // email,  password,  name,  description,  city,  address,  latitude,  longitude
            var clinic1 = CreateClinicUser("clinica1@asd.com", "clinica1@asd.com", "Clinica 1", "Clinica de Villa Bosch 1","Villa Bosch", "Jose Maria Bosch 951",-34.5883457, -58.5732785);
            var clinic2 = CreateClinicUser("clinica2@asd.com", "clinica2@asd.com", "Clinica 2", "Clinica de Moron 1", "Moron", "Yatay 600", -34.6548052, -58.6173822);
            var clinic3 = CreateClinicUser("clinica3@asd.com", "clinica3@asd.com", "Clinica 3", "Clinica de Villa Bosch 2", "Villa Bosch", "Julio Besada 6300", -34.5873598, -58.5852697);
            var clinic4 = CreateClinicUser("clinica4@asd.com", "clinica4@asd.com", "Clinica 4", "Clinica de Coronado 1", "Martin Coronado", "Azopardo 7672", -34.5855405, -58.6017834);

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
                new List<Clinic_WorkingHours> {
                    new Clinic_WorkingHours { DayNumber = DayOfWeek.Monday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(16, 0, 0) },
                    new Clinic_WorkingHours { DayNumber = DayOfWeek.Tuesday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(16, 0, 0) },
                    new Clinic_WorkingHours { DayNumber = DayOfWeek.Wednesday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(16, 0, 0) },
                    new Clinic_WorkingHours { DayNumber = DayOfWeek.Thursday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(16, 0, 0) },
                    new Clinic_WorkingHours { DayNumber = DayOfWeek.Friday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(16, 0, 0) },
                    new Clinic_WorkingHours { DayNumber = DayOfWeek.Saturday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(16, 0, 0) },
                    new Clinic_WorkingHours { DayNumber = DayOfWeek.Sunday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(16, 0, 0) },
                }, clinic1);

            var doctor2 = CreateDoctor("Christian", "Russo", 10, specialty1, subspecialty1, DoctorStateEnum.Active,
                new List<Clinic_WorkingHours> {
                    new Clinic_WorkingHours { DayNumber = DayOfWeek.Monday, Start = new TimeSpan(7, 0, 0), End = new TimeSpan(21, 0, 0) },
                    new Clinic_WorkingHours { DayNumber = DayOfWeek.Tuesday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(16, 0, 0) },
                    new Clinic_WorkingHours { DayNumber = DayOfWeek.Wednesday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(16, 0, 0) },
                    new Clinic_WorkingHours { DayNumber = DayOfWeek.Thursday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(16, 0, 0) },
                    new Clinic_WorkingHours { DayNumber = DayOfWeek.Friday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(16, 0, 0) },
                    new Clinic_WorkingHours { DayNumber = DayOfWeek.Saturday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(16, 0, 0) },
                    new Clinic_WorkingHours { DayNumber = DayOfWeek.Sunday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(16, 0, 0) },
                }, clinic1);


            var doctor3 = CreateDoctor("Sabrina", "Fillol", 10, specialty1, subspecialty1, DoctorStateEnum.Active,
                new List<Clinic_WorkingHours> {
                    new Clinic_WorkingHours { DayNumber = DayOfWeek.Monday, Start = new TimeSpan(7, 0, 0), End = new TimeSpan(21, 0, 0) },
                    new Clinic_WorkingHours { DayNumber = DayOfWeek.Tuesday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(16, 0, 0) },
                    new Clinic_WorkingHours { DayNumber = DayOfWeek.Wednesday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(16, 0, 0) },
                    new Clinic_WorkingHours { DayNumber = DayOfWeek.Thursday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(16, 0, 0) },
                    new Clinic_WorkingHours { DayNumber = DayOfWeek.Friday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(16, 0, 0) },
                    new Clinic_WorkingHours { DayNumber = DayOfWeek.Saturday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(16, 0, 0) },
                    new Clinic_WorkingHours { DayNumber = DayOfWeek.Sunday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(16, 0, 0) },
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

        private Clinic CreateClinicUser(string email, string password, string name, string description, string city, string address, double latitude, double longitude)
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

                var cityData = dbContext.Cities.FirstOrDefault(c => c.Name == city);

                if (cityData == null)
                {
                    cityData = CreateCity(city);
                }

                clinic = new Clinic
                {
                    Name = name,
                    Description = description,
                    CityId = cityData.Id,
                    Address = address,
                    Latitude = latitude,
                    Longitude = longitude,
                    Logo = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAFoAAAAoBAMAAACMbPD7AAAAG1BMVEXMzMyWlpbFxcWjo6OqqqqxsbGcnJy+vr63t7eN+fR5AAAACXBIWXMAAA7EAAAOxAGVKw4bAAAApElEQVQ4je2QsQrCQBBEJ5fLpt2AHxCJWCc2WkZFsTwx9kcQ0ypK6lR+t3eInWw6q3vVLrwdlgECgcAvVFXqy3dm7GvR1ubczMxnjjnZ7ESbclqmJZK6B54c3x6iHYGsslBdByyYMBft2BwZDLxcvuHIXUuoatu6bEwHFGDK5ewUhf8bJ4t7lhUjf9Nw8J2oduWW0U7Sq9ETX2Tvbaxr0Q4E/s8bo1sUV4qjWrAAAAAASUVORK5CYII=",
                    UserId = appUser.Id
                };

                dbContext.Clinics.Add(clinic);
                dbContext.SaveChanges();
            }

            return clinic;
        }

        private City CreateCity(string cityName)
        {
            City city;

            using (var dbContext = new ApplicationDbContext())
            {
                city = new City
                {
                    Name = cityName
                };

                dbContext.Cities.Add(city);
                dbContext.SaveChanges();
            }

            return city;
        }

        private Clinic_Specialty CreateSpecialty(string description, Clinic clinic)
        {
            Clinic_Specialty specialty;

            using (var dbContext = new ApplicationDbContext())
            {
                var specialtyData = new SpecialtyData
                {
                    Description = description
                };

                specialty = new Clinic_Specialty
                {
                    Data = specialtyData,
                    UserId = clinic.UserId
                };

                dbContext.Specialties.Add(specialtyData);
                dbContext.Clinic_Specialties.Add(specialty);
                dbContext.SaveChanges();
            }

            return specialty;
        }

        private Clinic_Subspecialty CreateSubspecialty(string description, Clinic_Specialty specialty, uint consultationLength, Clinic clinic)
        {
            Clinic_Subspecialty subspecialty;

            using (var dbContext = new ApplicationDbContext())
            {
                var subspecialtyData = new SubspecialtyData
                {
                    Description = description,
                    SpecialtyDataId = specialty.DataId
                };

                subspecialty = new Clinic_Subspecialty
                {
                    Data = subspecialtyData,
                    SpecialtyId = specialty.Id,
                    ConsultationLength = consultationLength,
                    UserId = clinic.UserId
                };

                dbContext.Subspecialties.Add(subspecialtyData);
                dbContext.Clinic_Subspecialties.Add(subspecialty);
                dbContext.SaveChanges();
            }

            return subspecialty;
        }

        private Clinic_Doctor CreateDoctor(string firstName, string lastName, uint consultationLength, Clinic_Specialty specialty, Clinic_Subspecialty subspecialty, DoctorStateEnum state, List<Clinic_WorkingHours> workingHours, Clinic clinic)
        {
            Clinic_Doctor doctor;

            using (var dbContext = new ApplicationDbContext())
            {
                doctor = new Clinic_Doctor
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

                dbContext.Clinic_Doctors.Add(doctor);
                dbContext.SaveChanges();
            }

            return doctor;
        }

        private Clinic_MedicalInsurance CreateMedicalInsurance(string description, Clinic clinic)
        {
            Clinic_MedicalInsurance medicalInsurance;

            using (var dbContext = new ApplicationDbContext())
            {
                var medicalInsuranceData = new MedicalInsuranceData
                {
                    Description = description
                };

                medicalInsurance = new Clinic_MedicalInsurance
                {
                    Data = medicalInsuranceData,
                    UserId = clinic.UserId
                };

                dbContext.MedicalInsurances.Add(medicalInsuranceData);
                dbContext.Clinic_MedicalInsurances.Add(medicalInsurance);
                dbContext.SaveChanges();
            }

            return medicalInsurance;
        }

        private Clinic_MedicalPlan CreateMedicalPlan(string description, Clinic_MedicalInsurance medicalInsurance, Clinic clinic)
        {
            Clinic_MedicalPlan medicalPlan;

            using (var dbContext = new ApplicationDbContext())
            {
                var medicalPlanData = new MedicalPlanData
                {
                    Description = description,
                    MedicalInsuranceDataId = medicalInsurance.DataId
                };

                medicalPlan = new Clinic_MedicalPlan
                {
                    Data = medicalPlanData,
                    MedicalInsuranceId = medicalInsurance.Id,
                    UserId = clinic.UserId
                };

                dbContext.MedicalPlans.Add(medicalPlanData);
                dbContext.Clinic_MedicalPlans.Add(medicalPlan);
                dbContext.SaveChanges();
            }

            return medicalPlan;
        }

        private Clinic_Client CreateClientUser(string email, string password)
        {
            Clinic_Client client;

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

                client = new Clinic_Client
                {
                    UserId = appUser.Id
                };

                dbContext.Clinic_Clients.Add(client);
                dbContext.SaveChanges();
            }

            return client;
        }

        private Clinic_Patient CreatePatient(string firstName, string lastName, string address, string dni, Clinic_MedicalPlan medicalPlan, Clinic_Client client, Clinic clinic)
        {
            Clinic_Patient patient;

            using (var dbContext = new ApplicationDbContext())
            {
                patient = new Clinic_Patient
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

                dbContext.Clinic_Patients.Add(patient);
                dbContext.SaveChanges();
            }

            return patient;
        }

        private Clinic_Appointment CreateAppointment(DateTime dateTime, Clinic_Doctor doctor, Clinic_Patient patient, AppointmentStateEnum state, Clinic_Rating rating, Clinic clinic)
        {
            Clinic_Appointment appointment;

            using (var dbContext = new ApplicationDbContext())
            {
                appointment = new Clinic_Appointment
                {
                    DateTime = dateTime,
                    DoctorId = doctor.Id,
                    PatientId = patient.Id,
                    State = state,
                    RatingId = rating?.Id ?? 0,
                    UserId = clinic.UserId
                };

                dbContext.Clinic_Appointments.Add(appointment);
                dbContext.SaveChanges();
            }

            return appointment;
        }

        private Clinic_Employee CreateEmployee(string email, string password, Clinic clinic)
        {
            Clinic_Employee employee;

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

                employee = new Clinic_Employee
                {
                    UserId = appUser.Id,
                    OwnerUserId = clinic.UserId
                };

                dbContext.Clinic_Employees.Add(employee);
                dbContext.SaveChanges();
            }

            return employee;
        }
    }
}
