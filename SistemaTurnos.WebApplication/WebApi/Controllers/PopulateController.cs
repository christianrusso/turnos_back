﻿using Microsoft.AspNetCore.Cors;
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
            var clinic1 = CreateClinicUser("clinica1@asd.com", "clinica1@asd.com", "Clinica 1", "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.","Villa Bosch", "Jose Maria Bosch 951",-34.5883457, -58.5732785);
            var clinic2 = CreateClinicUser("clinica2@asd.com", "clinica2@asd.com", "Clinica 2", "Clinica de Moron 1", "Moron", "Yatay 600", -34.6548052, -58.6173822);
            var clinic3 = CreateClinicUser("clinica3@asd.com", "clinica3@asd.com", "Clinica 3", "Clinica de Villa Bosch 2", "Villa Bosch", "Julio Besada 6300", -34.5873598, -58.5852697);
            

            // Creo empleados
            var employee1 = CreateEmployee("empleado1@asd.com", "empleado1@asd.com", clinic1);
            var employee2 = CreateEmployee("empleado2@asd.com", "empleado2@asd.com", clinic1);

            // Creo especialidades
            var specialty1 = CreateSpecialty("Kinesiologia", clinic1);
            var specialty2 = CreateSpecialty("Oftalmologia", clinic1);
            var specialty3 = CreateSpecialty("Farmacología", clinic1);

            var specialty4 = CreateSpecialty("Traumatología", clinic2);
            var specialty5 = CreateSpecialty("Especialidad 1", clinic2);
            var specialty6 = CreateSpecialty("Especialidad 2", clinic2);

            var specialty7 = CreateSpecialty("Especialidad 3", clinic3);
            var specialty8 = CreateSpecialty("Especialidad 4", clinic3);
            var specialty9 = CreateSpecialty("Especialidad 5", clinic3);

            // Creo subespecialidades
            var subspecialty1 = CreateSubspecialty("Subespecialidad 1", specialty1, 10, clinic1);
            var subspecialty2 = CreateSubspecialty("Subespecialidad 2", specialty1, 20, clinic1);
            var subspecialty3 = CreateSubspecialty("Subespecialidad 3", specialty2, 30, clinic1);
            var subspecialty4 = CreateSubspecialty("Subespecialidad 4", specialty2, 40, clinic1);
            var subspecialty5 = CreateSubspecialty("Subespecialidad 5", specialty3, 50, clinic1);
            var subspecialty6 = CreateSubspecialty("Subespecialidad 6", specialty3, 60, clinic1);

            var subspecialty7 = CreateSubspecialty("Subespecialidad 7", specialty4, 10, clinic2);
            var subspecialty8 = CreateSubspecialty("Subespecialidad 8", specialty4, 20, clinic2);
            var subspecialty9 = CreateSubspecialty("Subespecialidad 9", specialty5, 30, clinic2);
            var subspecialty10 = CreateSubspecialty("Subespecialidad 10", specialty6, 40, clinic2);
            var subspecialty11 = CreateSubspecialty("Subespecialidad 11", specialty6, 60, clinic2);


            var subspecialty12 = CreateSubspecialty("Subespecialidad 12", specialty7, 20, clinic3);
            var subspecialty13 = CreateSubspecialty("Subespecialidad 13", specialty7, 40, clinic3);
            var subspecialty14 = CreateSubspecialty("Subespecialidad 14", specialty7, 60, clinic3);
            var subspecialty15 = CreateSubspecialty("Subespecialidad 15", specialty8, 30, clinic3);
            var subspecialty16 = CreateSubspecialty("Subespecialidad 16", specialty9, 80, clinic3);

            // Creo doctores 
            var doctor1 = CreateDoctor("Fernando", "Gomez", 30, specialty2, null, DoctorStateEnum.Active,
                new List<Clinic_WorkingHours> {
                    new Clinic_WorkingHours { DayNumber = DayOfWeek.Monday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(16, 0, 0) },
                    new Clinic_WorkingHours { DayNumber = DayOfWeek.Tuesday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(16, 0, 0) },
                    new Clinic_WorkingHours { DayNumber = DayOfWeek.Wednesday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(16, 0, 0) },
                    new Clinic_WorkingHours { DayNumber = DayOfWeek.Thursday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(16, 0, 0) },
                    new Clinic_WorkingHours { DayNumber = DayOfWeek.Friday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(16, 0, 0) },
                    new Clinic_WorkingHours { DayNumber = DayOfWeek.Saturday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(16, 0, 0) },
                    new Clinic_WorkingHours { DayNumber = DayOfWeek.Sunday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(16, 0, 0) }
                }, clinic1);

            var doctor2 = CreateDoctor("Christian", "Russo", 10, specialty1, subspecialty1, DoctorStateEnum.Active,
                new List<Clinic_WorkingHours> {
                    new Clinic_WorkingHours { DayNumber = DayOfWeek.Monday, Start = new TimeSpan(7, 0, 0), End = new TimeSpan(21, 0, 0) },
                    new Clinic_WorkingHours { DayNumber = DayOfWeek.Tuesday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(16, 0, 0) },
                    new Clinic_WorkingHours { DayNumber = DayOfWeek.Wednesday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(16, 0, 0) },
                    new Clinic_WorkingHours { DayNumber = DayOfWeek.Thursday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(16, 0, 0) },
                    new Clinic_WorkingHours { DayNumber = DayOfWeek.Friday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(16, 0, 0) },
                    new Clinic_WorkingHours { DayNumber = DayOfWeek.Saturday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(16, 0, 0) },
                    new Clinic_WorkingHours { DayNumber = DayOfWeek.Sunday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(16, 0, 0) }
                }, clinic1);


            var doctor3 = CreateDoctor("Sabrina", "Fillol", 10, specialty1, subspecialty1, DoctorStateEnum.Active,
                new List<Clinic_WorkingHours> {
                    new Clinic_WorkingHours { DayNumber = DayOfWeek.Monday, Start = new TimeSpan(7, 0, 0), End = new TimeSpan(21, 0, 0) },
                    new Clinic_WorkingHours { DayNumber = DayOfWeek.Tuesday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(16, 0, 0) },
                    new Clinic_WorkingHours { DayNumber = DayOfWeek.Wednesday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(16, 0, 0) },
                    new Clinic_WorkingHours { DayNumber = DayOfWeek.Thursday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(16, 0, 0) },
                    new Clinic_WorkingHours { DayNumber = DayOfWeek.Friday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(16, 0, 0) },
                    new Clinic_WorkingHours { DayNumber = DayOfWeek.Saturday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(16, 0, 0) },
                    new Clinic_WorkingHours { DayNumber = DayOfWeek.Sunday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(16, 0, 0) }
                }, clinic1);


            var doctor4 = CreateDoctor("Pedro", "Perez", 30, specialty5, subspecialty9, DoctorStateEnum.Active,
                new List<Clinic_WorkingHours> {
                    new Clinic_WorkingHours { DayNumber = DayOfWeek.Monday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(20, 0, 0) },
                    new Clinic_WorkingHours { DayNumber = DayOfWeek.Tuesday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(20, 0, 0) },
                    new Clinic_WorkingHours { DayNumber = DayOfWeek.Wednesday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(20, 0, 0) },
                    new Clinic_WorkingHours { DayNumber = DayOfWeek.Thursday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(20, 0, 0) },
                    new Clinic_WorkingHours { DayNumber = DayOfWeek.Friday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(20, 0, 0) },
                    new Clinic_WorkingHours { DayNumber = DayOfWeek.Saturday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(20, 0, 0) },
                    new Clinic_WorkingHours { DayNumber = DayOfWeek.Sunday, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(20, 0, 0) }
                }, clinic2);

            var doctor5 = CreateDoctor("Eduardo", "Martinez", 10, specialty6, subspecialty11, DoctorStateEnum.Active,
                new List<Clinic_WorkingHours> {
                    new Clinic_WorkingHours { DayNumber = DayOfWeek.Monday, Start = new TimeSpan(9, 30, 0), End = new TimeSpan(17, 50, 0) },
                    new Clinic_WorkingHours { DayNumber = DayOfWeek.Tuesday, Start = new TimeSpan(9, 30, 0), End = new TimeSpan(17, 50, 0) },
                    new Clinic_WorkingHours { DayNumber = DayOfWeek.Wednesday, Start = new TimeSpan(9, 30, 0), End = new TimeSpan(17, 50, 0) },
                    new Clinic_WorkingHours { DayNumber = DayOfWeek.Thursday, Start = new TimeSpan(9, 30, 0), End = new TimeSpan(17, 50, 0) },
                    new Clinic_WorkingHours { DayNumber = DayOfWeek.Friday, Start = new TimeSpan(9, 30, 0), End = new TimeSpan(17, 50, 0) },
                    new Clinic_WorkingHours { DayNumber = DayOfWeek.Saturday, Start = new TimeSpan(9, 30, 0), End = new TimeSpan(17, 50, 0) },
                    new Clinic_WorkingHours { DayNumber = DayOfWeek.Sunday, Start = new TimeSpan(9, 30, 0), End = new TimeSpan(17, 50, 0) }
                }, clinic2);

            var doctor6 = CreateDoctor("Eliana", "Lint", 60, specialty8, subspecialty15, DoctorStateEnum.Active,
                new List<Clinic_WorkingHours> {
                    new Clinic_WorkingHours { DayNumber = DayOfWeek.Monday, Start = new TimeSpan(9, 30, 0), End = new TimeSpan(22, 30, 0) },
                    new Clinic_WorkingHours { DayNumber = DayOfWeek.Tuesday, Start = new TimeSpan(9, 30, 0), End = new TimeSpan(22, 30, 0) },
                    new Clinic_WorkingHours { DayNumber = DayOfWeek.Wednesday, Start = new TimeSpan(9, 30, 0), End = new TimeSpan(22, 30, 0) },
                    new Clinic_WorkingHours { DayNumber = DayOfWeek.Thursday, Start = new TimeSpan(9, 30, 0), End = new TimeSpan(22, 30, 0) },
                    new Clinic_WorkingHours { DayNumber = DayOfWeek.Friday, Start = new TimeSpan(9, 30, 0), End = new TimeSpan(22, 30, 0) },
                    new Clinic_WorkingHours { DayNumber = DayOfWeek.Saturday, Start = new TimeSpan(9, 30, 0), End = new TimeSpan(22, 30, 0) },
                    new Clinic_WorkingHours { DayNumber = DayOfWeek.Sunday, Start = new TimeSpan(9, 30, 0), End = new TimeSpan(22, 30, 0) }
                }, clinic3);

            var doctor7 = CreateDoctor("Daniela", "Disig", 30, specialty9, null, DoctorStateEnum.Active,
                new List<Clinic_WorkingHours> {
                    new Clinic_WorkingHours { DayNumber = DayOfWeek.Monday, Start = new TimeSpan(7, 30, 0), End = new TimeSpan(12, 30, 0) },
                    new Clinic_WorkingHours { DayNumber = DayOfWeek.Tuesday, Start = new TimeSpan(7, 30, 0), End = new TimeSpan(12, 30, 0) },
                    new Clinic_WorkingHours { DayNumber = DayOfWeek.Wednesday, Start = new TimeSpan(7, 30, 0), End = new TimeSpan(12, 30, 0) },
                    new Clinic_WorkingHours { DayNumber = DayOfWeek.Thursday, Start = new TimeSpan(7, 30, 0), End = new TimeSpan(12, 30, 0) },
                    new Clinic_WorkingHours { DayNumber = DayOfWeek.Friday, Start = new TimeSpan(7, 30, 0), End = new TimeSpan(12, 30, 0) },
                    new Clinic_WorkingHours { DayNumber = DayOfWeek.Saturday, Start = new TimeSpan(7, 30, 0), End = new TimeSpan(12, 30, 0) },
                    new Clinic_WorkingHours { DayNumber = DayOfWeek.Sunday, Start = new TimeSpan(7, 30, 0), End = new TimeSpan(12, 30, 0) },
                }, clinic3);

            // Creo obras sociales
            var medicalInsurance1 = CreateMedicalInsurance("OSDE", clinic1);
            var medicalInsurance2 = CreateMedicalInsurance("Galeno", clinic1);
            var medicalInsurance3 = CreateMedicalInsurance("Swiss Medical", clinic1);

            var medicalInsurance4 = CreateMedicalInsurance("Galeno", clinic2);
            var medicalInsurance5 = CreateMedicalInsurance("Swiss Medical", clinic2);

            var medicalInsurance6 = CreateMedicalInsurance("OSDE", clinic3);
            var medicalInsurance7 = CreateMedicalInsurance("Medlife", clinic3);
            var medicalInsurance8 = CreateMedicalInsurance("Swiss Medical", clinic3);

            // Creo planes de obras sociales
            var medicalPlan1 = CreateMedicalPlan("OSDE 210", medicalInsurance1, clinic1);
            var medicalPlan2 = CreateMedicalPlan("OSDE 310", medicalInsurance1, clinic1);
            var medicalPlan3 = CreateMedicalPlan("OSDE 410", medicalInsurance1, clinic1);
            var medicalPlan4 = CreateMedicalPlan("Galeno XL", medicalInsurance2, clinic1);
            var medicalPlan5 = CreateMedicalPlan("SM 10", medicalInsurance3, clinic1);
            var medicalPlan6 = CreateMedicalPlan("SM 20", medicalInsurance3, clinic1);
            var medicalPlan7 = CreateMedicalPlan("MEDI A", medicalInsurance7, clinic1);
            var medicalPlan8 = CreateMedicalPlan("MEDI B", medicalInsurance7, clinic1);
            var medicalPlan9 = CreateMedicalPlan("MEDI C", medicalInsurance7, clinic1);

            var medicalPlan10 = CreateMedicalPlan("Galeno XL", medicalInsurance2, clinic2);
            var medicalPlan11 = CreateMedicalPlan("SM 10", medicalInsurance3, clinic2);
            var medicalPlan12 = CreateMedicalPlan("SM 20", medicalInsurance3, clinic2);

            var medicalPlan13 = CreateMedicalPlan("OSDE 410", medicalInsurance1, clinic3);
            var medicalPlan14 = CreateMedicalPlan("SM 10", medicalInsurance3, clinic3);
            var medicalPlan15 = CreateMedicalPlan("SM 20", medicalInsurance3, clinic3);
            var medicalPlan16 = CreateMedicalPlan("MEDI A", medicalInsurance7, clinic3);
            var medicalPlan17 = CreateMedicalPlan("MEDI B", medicalInsurance7, clinic3);
            var medicalPlan18 = CreateMedicalPlan("MEDI C", medicalInsurance7, clinic3);

            // Creo clientes
            var client1 = CreateClientUser("cliente1@asd.com", "cliente1@asd.com");
            var client2 = CreateClientUser("cliente2@asd.com", "cliente2@asd.com");
            var client3 = CreateClientUser("cliente3@asd.com", "cliente3@asd.com");
            var client4 = CreateClientUser("cliente4@asd.com", "cliente4@asd.com");
            var client5 = CreateClientUser("cliente5@asd.com", "cliente5@asd.com");
            var client6 = CreateClientUser("cliente6@asd.com", "cliente6@asd.com");
            var client7 = CreateClientUser("cliente7@asd.com", "cliente7@asd.com");
            var client8 = CreateClientUser("cliente8@asd.com", "cliente8@asd.com");
            var client9 = CreateClientUser("cliente9@asd.com", "cliente9@asd.com");
            var client10 = CreateClientUser("cliente10@asd.com", "cliente10@asd.com");
            var client11 = CreateClientUser("cliente11@asd.com", "cliente11@asd.com");
            var client12 = CreateClientUser("cliente12@asd.com", "cliente12@asd.com");
            var client13 = CreateClientUser("cliente13@asd.com", "cliente13@asd.com");
            var client14 = CreateClientUser("cliente14@asd.com", "cliente14@asd.com");
            var client15 = CreateClientUser("cliente15@asd.com", "cliente15@asd.com");
            var client16 = CreateClientUser("cliente16@asd.com", "cliente16@asd.com");
            var client17 = CreateClientUser("cliente17@asd.com", "cliente17@asd.com");
            var client18 = CreateClientUser("cliente18@asd.com", "cliente18@asd.com");
            var client19 = CreateClientUser("cliente19@asd.com", "cliente19@asd.com");
            var client20 = CreateClientUser("cliente20@asd.com", "cliente20@asd.com");

            // Creo pacientes
            var patient1 = CreatePatient("Pedro", "Gomez", "qwerty 1", "1000001", medicalPlan1, client1, clinic1);
            var patient2 = CreatePatient("Juan", "Martinez", "qwerty 2", "1000002", medicalPlan3, client2, clinic1);
            var patient3 = CreatePatient("Martin", "Violante", "qwerty 3", "1000003", medicalPlan6, client3, clinic1);
            var patient4 = CreatePatient("Leandro", "Lagos", "qwerty 4", "1000004", medicalPlan7, client4, clinic1);
            var patient5 = CreatePatient("Sebastian", "Veliz", "qwerty 5", "1000005", medicalPlan9, client5, clinic1);

            var patient6 = CreatePatient("Maria", "Light", "qwerty 6", "1000006", medicalPlan10, client6, clinic2);
            var patient7 = CreatePatient("Clara", "Florin", "qwerty 7", "1000007", medicalPlan10, client7, clinic2);
            var patient8 = CreatePatient("Daniela", "Brign", "qwerty 8", "1000008", medicalPlan10, client8, clinic2);
            var patient9 = CreatePatient("Sara", "Coronel", "qwerty 9", "1000009", medicalPlan11, client9, clinic2);
            var patient10 = CreatePatient("Matias", "Marquez", "qwerty 10", "1000010", medicalPlan11, client10, clinic2);

            var patient11 = CreatePatient("Daniel", "Perez", "qwerty 11", "1000011", medicalPlan13, client11, clinic3);
            var patient12 = CreatePatient("Javier", "Rito", "qwerty 12", "1000012", medicalPlan13, client12,clinic3);
            var patient13 = CreatePatient("Luis", "Salas", "qwerty 13", "1000013", medicalPlan15, client13, clinic3);
            var patient14 = CreatePatient("Ezequiel", "Morzt", "qwerty 14", "1000014", medicalPlan16, client14, clinic3);
            var patient15 = CreatePatient("Manuela", "Gomez", "qwerty 15", "1000015", medicalPlan18, client15, clinic3);

            // Creo turnos
            var appointment1 = CreateAppointment(DateTime.Today.AddDays(-5).AddHours(8).AddMinutes(30), doctor1, patient1, AppointmentStateEnum.Reserved, null, clinic1);
            var appointment2 = CreateAppointment(DateTime.Today.AddDays(-3).AddHours(10).AddMinutes(30), doctor1, patient2, AppointmentStateEnum.Reserved, null, clinic1);
            var appointment3 = CreateAppointment(DateTime.Today.AddDays(-3).AddHours(12).AddMinutes(0), doctor1, patient3, AppointmentStateEnum.Reserved, null, clinic1);
            var appointment4 = CreateAppointment(DateTime.Today.AddDays(-2).AddHours(15).AddMinutes(30), doctor1, patient5, AppointmentStateEnum.Reserved, null, clinic1);
            var appointment5 = CreateAppointment(DateTime.Today.AddDays(-1).AddHours(8).AddMinutes(40), doctor2, patient3, AppointmentStateEnum.Reserved, null, clinic1);
            var appointment6 = CreateAppointment(DateTime.Today.AddDays(0).AddHours(8).AddMinutes(50), doctor2, patient1, AppointmentStateEnum.Reserved, null, clinic1);
            var appointment7 = CreateAppointment(DateTime.Today.AddDays(0).AddHours(9).AddMinutes(20), doctor2, patient2, AppointmentStateEnum.Reserved, null, clinic1);
            var appointment8 = CreateAppointment(DateTime.Today.AddDays(0).AddHours(9).AddMinutes(30), doctor2, patient3, AppointmentStateEnum.Reserved, null, clinic1);
            var appointment9 = CreateAppointment(DateTime.Today.AddDays(0).AddHours(9).AddMinutes(50), doctor2, patient4, AppointmentStateEnum.Reserved, null, clinic1);
            var appointment10 = CreateAppointment(DateTime.Today.AddDays(1).AddHours(9).AddMinutes(30), doctor1, patient3, AppointmentStateEnum.Reserved, null, clinic1);
            var appointment11 = CreateAppointment(DateTime.Today.AddDays(1).AddHours(14).AddMinutes(20), doctor2, patient5, AppointmentStateEnum.Reserved, null, clinic1);
            var appointment12 = CreateAppointment(DateTime.Today.AddDays(1).AddHours(15).AddMinutes(0), doctor2, patient2, AppointmentStateEnum.Reserved, null, clinic1);
            var appointment13 = CreateAppointment(DateTime.Today.AddDays(2).AddHours(16).AddMinutes(0), doctor1, patient1, AppointmentStateEnum.Reserved, null, clinic1);
            var appointment14 = CreateAppointment(DateTime.Today.AddDays(3).AddHours(9).AddMinutes(20), doctor2, patient4, AppointmentStateEnum.Reserved, null, clinic1);
            var appointment15 = CreateAppointment(DateTime.Today.AddDays(4).AddHours(16).AddMinutes(40), doctor2, patient5, AppointmentStateEnum.Reserved, null, clinic1);
            var appointment16 = CreateAppointment(DateTime.Today.AddDays(4).AddHours(16).AddMinutes(50), doctor2, patient2, AppointmentStateEnum.Reserved, null, clinic1);
            var appointment17 = CreateAppointment(DateTime.Today.AddDays(4).AddHours(17).AddMinutes(0), doctor2, patient3, AppointmentStateEnum.Reserved, null, clinic1);
            var appointment18 = CreateAppointment(DateTime.Today.AddDays(4).AddHours(17).AddMinutes(10), doctor2, patient4, AppointmentStateEnum.Reserved, null, clinic1);
            var appointment19 = CreateAppointment(DateTime.Today.AddDays(4).AddHours(17).AddMinutes(20), doctor2, patient5, AppointmentStateEnum.Reserved, null, clinic1);
            var appointment20 = CreateAppointment(DateTime.Today.AddDays(4).AddHours(17).AddMinutes(40), doctor2, patient1, AppointmentStateEnum.Reserved, null, clinic1);

            var appointment21 = CreateAppointment(DateTime.Today.AddDays(-10).AddHours(8).AddMinutes(0), doctor4, patient6, AppointmentStateEnum.Reserved, null, clinic2);
            var appointment22 = CreateAppointment(DateTime.Today.AddDays(-9).AddHours(9).AddMinutes(30), doctor4, patient7, AppointmentStateEnum.Reserved, null, clinic2);
            var appointment23 = CreateAppointment(DateTime.Today.AddDays(-8).AddHours(10).AddMinutes(0), doctor4, patient8, AppointmentStateEnum.Reserved, null, clinic2);
            var appointment24 = CreateAppointment(DateTime.Today.AddDays(-7).AddHours(11).AddMinutes(30), doctor4, patient9, AppointmentStateEnum.Reserved, null, clinic2);
            var appointment25 = CreateAppointment(DateTime.Today.AddDays(-6).AddHours(12).AddMinutes(0), doctor4, patient10, AppointmentStateEnum.Reserved, null, clinic2);
            var appointment26 = CreateAppointment(DateTime.Today.AddDays(-5).AddHours(13).AddMinutes(30), doctor4, patient6, AppointmentStateEnum.Reserved, null, clinic2);
            var appointment27 = CreateAppointment(DateTime.Today.AddDays(-4).AddHours(14).AddMinutes(0), doctor4, patient6, AppointmentStateEnum.Reserved, null, clinic2);
            var appointment28 = CreateAppointment(DateTime.Today.AddDays(-3).AddHours(15).AddMinutes(30), doctor4, patient7, AppointmentStateEnum.Reserved, null, clinic2);
            var appointment29 = CreateAppointment(DateTime.Today.AddDays(-2).AddHours(16).AddMinutes(0), doctor4, patient8, AppointmentStateEnum.Reserved, null, clinic2);
            var appointment30 = CreateAppointment(DateTime.Today.AddDays(-1).AddHours(17).AddMinutes(30), doctor4, patient9, AppointmentStateEnum.Reserved, null, clinic2);
            var appointment31 = CreateAppointment(DateTime.Today.AddDays(0).AddHours(18).AddMinutes(0), doctor4, patient9, AppointmentStateEnum.Reserved, null, clinic2);
            var appointment32 = CreateAppointment(DateTime.Today.AddDays(1).AddHours(19).AddMinutes(30), doctor4, patient10, AppointmentStateEnum.Reserved, null, clinic2);
            var appointment33 = CreateAppointment(DateTime.Today.AddDays(2).AddHours(20).AddMinutes(0), doctor4, patient6, AppointmentStateEnum.Reserved, null, clinic2);
            var appointment34 = CreateAppointment(DateTime.Today.AddDays(3).AddHours(9).AddMinutes(30), doctor5, patient7, AppointmentStateEnum.Reserved, null, clinic2);
            var appointment35 = CreateAppointment(DateTime.Today.AddDays(4).AddHours(10).AddMinutes(40), doctor5, patient8, AppointmentStateEnum.Reserved, null, clinic2);
            var appointment36 = CreateAppointment(DateTime.Today.AddDays(5).AddHours(11).AddMinutes(50), doctor5, patient8, AppointmentStateEnum.Reserved, null, clinic2);
            var appointment37 = CreateAppointment(DateTime.Today.AddDays(6).AddHours(12).AddMinutes(0), doctor5, patient8, AppointmentStateEnum.Reserved, null, clinic2);
            var appointment38 = CreateAppointment(DateTime.Today.AddDays(7).AddHours(13).AddMinutes(10), doctor5, patient9, AppointmentStateEnum.Reserved, null, clinic2);
            var appointment39 = CreateAppointment(DateTime.Today.AddDays(8).AddHours(14).AddMinutes(20), doctor5, patient9, AppointmentStateEnum.Reserved, null, clinic2);
            var appointment40 = CreateAppointment(DateTime.Today.AddDays(9).AddHours(15).AddMinutes(30), doctor5, patient9, AppointmentStateEnum.Reserved, null, clinic2);

            var appointment41 = CreateAppointment(DateTime.Today.AddDays(-2).AddHours(10).AddMinutes(30), doctor6, patient15, AppointmentStateEnum.Reserved, null, clinic3);
            var appointment42 = CreateAppointment(DateTime.Today.AddDays(-2).AddHours(7).AddMinutes(30), doctor7, patient14, AppointmentStateEnum.Reserved, null, clinic3);
            var appointment43 = CreateAppointment(DateTime.Today.AddDays(-2).AddHours(16).AddMinutes(30), doctor6, patient14, AppointmentStateEnum.Reserved, null, clinic3);
            var appointment44 = CreateAppointment(DateTime.Today.AddDays(-2).AddHours(12).AddMinutes(0), doctor7, patient12, AppointmentStateEnum.Reserved, null, clinic3);
            var appointment45 = CreateAppointment(DateTime.Today.AddDays(-2).AddHours(22).AddMinutes(30), doctor6, patient11, AppointmentStateEnum.Reserved, null, clinic3);
            var appointment46 = CreateAppointment(DateTime.Today.AddDays(-1).AddHours(8).AddMinutes(0), doctor7, patient11, AppointmentStateEnum.Reserved, null, clinic3);
            var appointment47 = CreateAppointment(DateTime.Today.AddDays(-1).AddHours(14).AddMinutes(30), doctor6, patient13, AppointmentStateEnum.Reserved, null, clinic3);
            var appointment48 = CreateAppointment(DateTime.Today.AddDays(-1).AddHours(9).AddMinutes(0), doctor7, patient3, AppointmentStateEnum.Reserved, null, clinic3);
            var appointment49 = CreateAppointment(DateTime.Today.AddDays(-1).AddHours(16).AddMinutes(30), doctor6, patient13, AppointmentStateEnum.Reserved, null, clinic3);
            var appointment50 = CreateAppointment(DateTime.Today.AddDays(-1).AddHours(11).AddMinutes(0), doctor7, patient14, AppointmentStateEnum.Reserved, null, clinic3);
            var appointment51 = CreateAppointment(DateTime.Today.AddDays(0).AddHours(18).AddMinutes(30), doctor6, patient15, AppointmentStateEnum.Reserved, null, clinic3);
            var appointment52 = CreateAppointment(DateTime.Today.AddDays(0).AddHours(12).AddMinutes(30), doctor7, patient11, AppointmentStateEnum.Reserved, null, clinic3);
            var appointment53 = CreateAppointment(DateTime.Today.AddDays(0).AddHours(20).AddMinutes(30), doctor6, patient11, AppointmentStateEnum.Reserved, null, clinic3);
            var appointment54 = CreateAppointment(DateTime.Today.AddDays(0).AddHours(9).AddMinutes(30), doctor7, patient12, AppointmentStateEnum.Reserved, null, clinic3);
            var appointment55 = CreateAppointment(DateTime.Today.AddDays(0).AddHours(10).AddMinutes(30), doctor6, patient12, AppointmentStateEnum.Reserved, null, clinic3);
            var appointment56 = CreateAppointment(DateTime.Today.AddDays(1).AddHours(10).AddMinutes(30), doctor7, patient12, AppointmentStateEnum.Reserved, null, clinic3);
            var appointment57 = CreateAppointment(DateTime.Today.AddDays(1).AddHours(10).AddMinutes(30), doctor6, patient14, AppointmentStateEnum.Reserved, null, clinic3);
            var appointment58 = CreateAppointment(DateTime.Today.AddDays(1).AddHours(11).AddMinutes(0), doctor7, patient13, AppointmentStateEnum.Reserved, null, clinic3);
            var appointment59 = CreateAppointment(DateTime.Today.AddDays(1).AddHours(11).AddMinutes(30), doctor6, patient15, AppointmentStateEnum.Reserved, null, clinic3);
            var appointment60 = CreateAppointment(DateTime.Today.AddDays(2).AddHours(11).AddMinutes(30), doctor7, patient11, AppointmentStateEnum.Reserved, null, clinic3);
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
                var specialtyData = dbContext.Specialties.FirstOrDefault(s => s.Description == description);

                if (specialtyData == null)
                {
                    specialtyData = new SpecialtyData
                    {
                        Description = description
                    };

                    dbContext.Specialties.Add(specialtyData);
                }

                dbContext.SaveChanges();

                specialty = new Clinic_Specialty
                {
                    DataId = specialtyData.Id,
                    UserId = clinic.UserId
                };

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
                var subspecialtyData = dbContext.Subspecialties.FirstOrDefault(sp => sp.Description == description);

                if (subspecialtyData == null)
                {
                    subspecialtyData = new SubspecialtyData
                    {
                        Description = description,
                        SpecialtyDataId = specialty.DataId
                    };

                    dbContext.Subspecialties.Add(subspecialtyData);
                }

                dbContext.SaveChanges();

                subspecialty = new Clinic_Subspecialty
                {
                    DataId = subspecialtyData.Id,
                    SpecialtyId = specialty.Id,
                    ConsultationLength = consultationLength,
                    UserId = clinic.UserId
                };

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
                var medicalInsuranceData = dbContext.MedicalInsurances.FirstOrDefault(mi => mi.Description == description);

                if (medicalInsuranceData == null)
                {
                    medicalInsuranceData = new MedicalInsuranceData
                    {
                        Description = description
                    };

                    dbContext.MedicalInsurances.Add(medicalInsuranceData);
                }


                medicalInsurance = new Clinic_MedicalInsurance
                {
                    DataId = medicalInsuranceData.Id,
                    UserId = clinic.UserId
                };
                
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
                var medicalPlanData = dbContext.MedicalPlans.FirstOrDefault(mp => mp.Description == description);

                if (medicalPlanData == null)
                {
                    medicalPlanData = new MedicalPlanData
                    {
                        Description = description,
                        MedicalInsuranceDataId = medicalInsurance.DataId
                    };

                    dbContext.MedicalPlans.Add(medicalPlanData);
                }

                medicalPlan = new Clinic_MedicalPlan
                {
                    DataId = medicalPlanData.Id,
                    MedicalInsuranceId = medicalInsurance.Id,
                    UserId = clinic.UserId
                };

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
