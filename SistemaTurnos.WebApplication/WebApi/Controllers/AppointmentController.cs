﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SistemaTurnos.Commons.Authorization;
using SistemaTurnos.Commons.Exceptions;
using SistemaTurnos.Database;
using SistemaTurnos.Database.ClinicModel;
using SistemaTurnos.Database.Enums;
using SistemaTurnos.Database.Model;
using SistemaTurnos.WebApplication.WebApi.Dto;
using SistemaTurnos.WebApplication.WebApi.Dto.Appointment;
using SistemaTurnos.WebApplication.WebApi.Dto.Email;
using SistemaTurnos.WebApplication.WebApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SistemaTurnos.WebApplication.WebApi.Controllers
{
    [Route("Api/[controller]/[action]")]
    [Produces("application/json")]
    [EnableCors("AnyOrigin")]
    public class AppointmentController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly EmailService emailService = new EmailService();

        public AppointmentController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        /// <summary>
        /// Dada un doctor y una clinica (no obligatorio) obtiene todos los turnos disponibles desde ese dia.
        /// </summary>
        /// <param name="getAppointmentDto"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public List<DateTime> GetAllAvailablesFromDay([FromBody] GetAppointmentDto getAppointmentDto)
        {
            using (var dbContext = new ApplicationDbContext())
            {

                var userId = GetUserId();

                Clinic_Doctor doctor = dbContext.Clinic_Doctors.FirstOrDefault(d => d.Id == getAppointmentDto.DoctorId && d.UserId == userId);

                if (doctor == null)
                {
                    throw new BadRequestException();
                }

                var res = new List<DateTime>();
                for (int i = 0; i < 15; i++)
                {
                    foreach (var datetime in doctor.GetAllAvailablesForDay(getAppointmentDto.Day.AddDays(i)))
                    {
                        res.Add(datetime);
                    }
                }

                return res;
            }
        }

        /// <summary>
        /// Dada un doctor y una clinica (no obligatorio) obtiene todos los turnos disponibles de ese dia en particular.
        /// </summary>
        /// <param name="getAppointmentDto"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = Roles.AdministratorAndEmployeeAndClient)]
        public List<DateTime> GetAllAvailablesForDay([FromBody] GetAppointmentDto getAppointmentDto)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                
                int? userId = GetUserId();

                if(getAppointmentDto.ClinicId != null)
                    userId = getAppointmentDto.ClinicId;

                var doctor = dbContext.Clinic_Doctors.FirstOrDefault(d => d.Id == getAppointmentDto.DoctorId);

                if (doctor == null)
                {
                    throw new BadRequestException();
                }

                return doctor.GetAllAvailablesForDay(getAppointmentDto.Day);
            }
        }

        /// <summary>
        /// Siendo Clinica, Agenda un turno para una persona que no sea cliente. Es necesario pasar todos los datos. Lo convierte en Cliente, tambien en paciente y registra turno.
        /// </summary>
        /// <param name="getAppointmentDto"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public void RequestAppointmentForNonClient([FromBody] RequestAppointmentForNonClientDto requestAppointmentDto)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

                if (requestAppointmentDto.Day.Date < DateTime.Today.Date)
                {
                    throw new BadRequestException(ExceptionMessages.AppointmentCantBeRequested);
                }

                var doctor = dbContext.Clinic_Doctors.FirstOrDefault(d => d.Id == requestAppointmentDto.DoctorId && d.UserId == userId);

                if (doctor == null)
                {
                    throw new BadRequestException();
                }

                var medicalPlan = dbContext.Clinic_MedicalPlans.FirstOrDefault(mp => mp.Id == requestAppointmentDto.MedicalPlanId);

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
                    UserName = requestAppointmentDto.Email,
                    Email = requestAppointmentDto.Email
                };

                var result = _userManager.CreateAsync(user, requestAppointmentDto.Password).Result;

                if (!result.Succeeded)
                {
                    throw new ApplicationException(ExceptionMessages.UsernameAlreadyExists);
                }

                var appUser = _userManager.Users.SingleOrDefault(au => au.Email == requestAppointmentDto.Email);

                result = _userManager.AddToRoleAsync(appUser, Roles.Client).Result;

                if (!result.Succeeded)
                {
                    throw new ApplicationException(ExceptionMessages.InternalServerError);
                }

                var client = new SystemClient
                {
                    UserId = appUser.Id,
                    FirstName = requestAppointmentDto.FirstName,
                    LastName = requestAppointmentDto.LastName,
                    Address = requestAppointmentDto.Address,
                    PhoneNumber = requestAppointmentDto.PhoneNumber,
                    Dni = requestAppointmentDto.Dni,
                };

                dbContext.Clients.Add(client);
                dbContext.SaveChanges();

                var patient = new Clinic_Patient
                {
                    UserId = userId,
                    ClientId = client.Id,
                    MedicalPlanId = requestAppointmentDto.MedicalPlanId
                };

                dbContext.Clinic_Patients.Add(patient);
                dbContext.SaveChanges();

                var availableAppointments = doctor.GetAllAvailablesForDay(requestAppointmentDto.Day.Date);

                var appointment = new DateTime(
                        requestAppointmentDto.Day.Year,
                        requestAppointmentDto.Day.Month,
                        requestAppointmentDto.Day.Day,
                        requestAppointmentDto.Time.Hour,
                        requestAppointmentDto.Time.Minute,
                        requestAppointmentDto.Time.Second
                    );

                if (!availableAppointments.Contains(appointment))
                {
                    throw new BadRequestException(ExceptionMessages.AppointmentAlreadyTaken);
                }

                dbContext.Clinic_Appointments.Add(new Clinic_Appointment
                {
                    DoctorId = requestAppointmentDto.DoctorId,
                    Doctor = doctor,
                    DateTime = appointment,
                    State = AppointmentStateEnum.Reserved,
                    Source = AppointmentSourceEnum.Panel,
                    PatientId = patient.Id,
                    UserId = userId
                });

                dbContext.SaveChanges();
            }
        }

        /// <summary>
        /// Siendo Clinica, Agenda un turno para una persona que sea Cliente. Registra el paciente y registra el turno.
        /// </summary>
        /// <param name="getAppointmentDto"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = Roles.AdministratorAndEmployee)]
        public void RequestAppointmentForClient([FromBody] RequestAppointmentForClientDto requestAppointmentDto)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

                if (requestAppointmentDto.Day.Date < DateTime.Today.Date)
                {
                    throw new BadRequestException(ExceptionMessages.AppointmentCantBeRequested);
                }

                var doctor = dbContext.Clinic_Doctors.FirstOrDefault(d => d.Id == requestAppointmentDto.DoctorId && d.UserId == userId);

                if (doctor == null)
                {
                    throw new BadRequestException();
                }

                var client = dbContext.Clients.FirstOrDefault(c => c.Id == requestAppointmentDto.ClientId);

                if (client == null)
                {
                    throw new BadRequestException();
                }

                var medicalPlan = dbContext.Clinic_MedicalPlans.FirstOrDefault(mp => mp.Id == requestAppointmentDto.MedicalPlanId);

                if (medicalPlan == null)
                {
                    throw new BadRequestException();
                }

                var patient = dbContext.Clinic_Patients.FirstOrDefault(p => p.ClientId == client.Id && p.UserId == userId);

                if (patient != null)
                {
                    throw new BadRequestException();
                }

                patient = new Clinic_Patient
                {

                    UserId = userId,
                    ClientId = requestAppointmentDto.ClientId,
                    MedicalPlanId = requestAppointmentDto.MedicalPlanId
                };

                dbContext.Clinic_Patients.Add(patient);

                var availableAppointments = doctor.GetAllAvailablesForDay(requestAppointmentDto.Day.Date);

                var appointment = new DateTime(
                        requestAppointmentDto.Day.Year,
                        requestAppointmentDto.Day.Month,
                        requestAppointmentDto.Day.Day,
                        requestAppointmentDto.Time.Hour,
                        requestAppointmentDto.Time.Minute,
                        requestAppointmentDto.Time.Second
                    );

                if (!availableAppointments.Contains(appointment))
                {
                    throw new BadRequestException();
                }

                dbContext.Clinic_Appointments.Add(new Clinic_Appointment
                {
                    DoctorId = requestAppointmentDto.DoctorId,
                    Doctor = doctor,
                    DateTime = appointment,
                    State = AppointmentStateEnum.Reserved,
                    Source = AppointmentSourceEnum.Panel,
                    PatientId = patient.Id,
                    UserId = userId
                });

                dbContext.SaveChanges();
            }
        }

        /// <summary>
        /// Siendo Clinica, Agenda un turno para una persona que sea Paciente. Registra el turno.
        /// </summary>
        /// <param name="getAppointmentDto"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = Roles.AdministratorAndEmployee)]
        public void RequestAppointmentForPatient([FromBody] RequestAppointmentForPatientDto requestAppointmentDto)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

                if (requestAppointmentDto.Day.Date < DateTime.Today.Date)
                {
                    throw new BadRequestException(ExceptionMessages.AppointmentCantBeRequested);
                }

                var doctor = dbContext.Clinic_Doctors.FirstOrDefault(d => d.Id == requestAppointmentDto.DoctorId && d.UserId == userId);

                if (doctor == null)
                {
                    throw new BadRequestException();
                }

                var patient = dbContext.Clinic_Patients.FirstOrDefault(p => p.Id == requestAppointmentDto.PatientId && p.UserId == userId);

                if (patient == null)
                {
                    throw new BadRequestException();
                }

                var availableAppointments = doctor.GetAllAvailablesForDay(requestAppointmentDto.Day.Date);

                var appointment = new DateTime(
                        requestAppointmentDto.Day.Year,
                        requestAppointmentDto.Day.Month,
                        requestAppointmentDto.Day.Day,
                        requestAppointmentDto.Time.Hour,
                        requestAppointmentDto.Time.Minute,
                        requestAppointmentDto.Time.Second
                    );

                if (!availableAppointments.Contains(appointment))
                {
                    throw new BadRequestException(ExceptionMessages.AppointmentAlreadyTaken);
                }

                dbContext.Clinic_Appointments.Add(new Clinic_Appointment
                {
                    DoctorId = requestAppointmentDto.DoctorId,
                    Doctor = doctor,
                    DateTime = appointment,
                    State = AppointmentStateEnum.Reserved,
                    Source = AppointmentSourceEnum.Panel,
                    PatientId = patient.Id,
                    UserId = userId
                });

                dbContext.SaveChanges();
            }
        }

        /// <summary>
        /// Siendo Cliente, Agenda un turno, Asocia este cliente con la clinica
        /// </summary>
        /// <param name="getAppointmentDto"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = Roles.Client)]
        public void RequestAppointmentByClient([FromBody] RequestAppointmentByClientDto requestAppointmentDto)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

                if (requestAppointmentDto.Day.Date < DateTime.Today.Date)
                {
                    throw new BadRequestException(ExceptionMessages.AppointmentCantBeRequested);
                }

                var clinic = dbContext.Clinics.FirstOrDefault(c => c.Id == requestAppointmentDto.ClinicId);

                if (clinic == null)
                {
                    throw new BadRequestException();
                }

                var doctor = dbContext.Clinic_Doctors.FirstOrDefault(d => d.Id == requestAppointmentDto.DoctorId && d.UserId == clinic.UserId);

                if (doctor == null)
                {
                    throw new BadRequestException();
                }

                var client = dbContext.Clients.FirstOrDefault(c => c.UserId == userId);

                if (client == null)
                {
                    throw new BadRequestException();
                }

                var medicalPlan = dbContext.Clinic_MedicalPlans.FirstOrDefault(mp => mp.Id == requestAppointmentDto.MedicalPlanId && mp.UserId == clinic.UserId);

                if (medicalPlan == null)
                {
                    throw new BadRequestException();
                }

                var patient = dbContext.Clinic_Patients.FirstOrDefault(p => p.ClientId == client.Id && p.UserId == clinic.UserId);

                if (patient != null)
                {
                    throw new BadRequestException();
                }

                patient = new Clinic_Patient
                {
                    UserId = clinic.UserId,
                    ClientId = client.Id,
                    MedicalPlanId = requestAppointmentDto.MedicalPlanId
                };

                dbContext.Clinic_Patients.Add(patient);

                var availableAppointments = doctor.GetAllAvailablesForDay(requestAppointmentDto.Day.Date);

                var appointment = new DateTime(
                        requestAppointmentDto.Day.Year,
                        requestAppointmentDto.Day.Month,
                        requestAppointmentDto.Day.Day,
                        requestAppointmentDto.Time.Hour,
                        requestAppointmentDto.Time.Minute,
                        requestAppointmentDto.Time.Second
                    );

                if (!availableAppointments.Contains(appointment))
                {
                    throw new BadRequestException(ExceptionMessages.AppointmentAlreadyTaken);
                }

                dbContext.Clinic_Appointments.Add(new Clinic_Appointment
                {
                    DoctorId = requestAppointmentDto.DoctorId,
                    Doctor = doctor,
                    DateTime = appointment,
                    State = AppointmentStateEnum.Reserved,
                    Source = (AppointmentSourceEnum) requestAppointmentDto.Source,
                    PatientId = patient.Id,
                    UserId = clinic.UserId
                });

                dbContext.SaveChanges();
            }
        }

        /// <summary>
        /// Siendo Paciente, Agenda un turno
        /// </summary>
        /// <param name="getAppointmentDto"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = Roles.Client)]
        public void RequestAppointmentByPatient([FromBody] RequestAppointmentByPatientDto requestAppointmentDto)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

                if (requestAppointmentDto.Day.Date < DateTime.Today.Date)
                {
                    throw new BadRequestException(ExceptionMessages.AppointmentCantBeRequested);
                }

                var clinic = dbContext.Clinics.FirstOrDefault(c => c.Id == requestAppointmentDto.ClinicId);

                if (clinic == null)
                {
                    throw new BadRequestException();
                }

                var doctor = dbContext.Clinic_Doctors.FirstOrDefault(d => d.Id == requestAppointmentDto.DoctorId && d.UserId == clinic.UserId);

                if (doctor == null)
                {
                    throw new BadRequestException();
                }

                var client = dbContext.Clients.FirstOrDefault(c => c.UserId == userId);
                var patient = dbContext.Clinic_Patients.FirstOrDefault(p => p.ClientId == client.Id && p.UserId == clinic.UserId);

                if (patient == null)
                {
                    throw new BadRequestException();
                }

                var availableAppointments = doctor.GetAllAvailablesForDay(requestAppointmentDto.Day.Date);

                var appointment = new DateTime(
                        requestAppointmentDto.Day.Year,
                        requestAppointmentDto.Day.Month,
                        requestAppointmentDto.Day.Day,
                        requestAppointmentDto.Time.Hour,
                        requestAppointmentDto.Time.Minute,
                        requestAppointmentDto.Time.Second
                    );

                if (!availableAppointments.Contains(appointment))
                {
                    throw new BadRequestException(ExceptionMessages.AppointmentAlreadyTaken);
                }

                dbContext.Clinic_Appointments.Add(new Clinic_Appointment
                {
                    DoctorId = requestAppointmentDto.DoctorId,
                    Doctor = doctor,
                    DateTime = appointment,
                    State = AppointmentStateEnum.Reserved,
                    Source = (AppointmentSourceEnum)requestAppointmentDto.Source,
                    PatientId = patient.Id,
                    UserId = clinic.UserId
                });

                dbContext.SaveChanges();
            }
        }

        /// <summary>
        /// Siendo Clinica, Cancela un turno
        /// </summary>
        /// <param name="getAppointmentDto"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = Roles.AdministratorAndEmployee)]
        public void CancelAppointmentByClinic([FromBody] CancelAppointmentDto cancelAppointmentDto)
        {
            var emailMessage = new EmailDto();

            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

                var appointment = dbContext.Clinic_Appointments.FirstOrDefault(a => a.Id == cancelAppointmentDto.Id && a.UserId == userId);

                if (appointment == null)
                {
                    throw new BadRequestException();
                }

                var clinic = dbContext.Clinics.FirstOrDefault(c => c.UserId == userId);

                if (clinic == null)
                {
                    throw new BadRequestException();
                }
                
                var maxCancelDateTime = appointment.DateTime.AddHours(-24);

                if (DateTime.Now >= maxCancelDateTime)
                {
                    throw new BadRequestException(ExceptionMessages.AppointmentCantBeCanceled);
                }

                string template = "<html lang='en'> <head> <meta charset='UTF-8'> <meta http-equiv='X-UA-Compatible' content='IE=edge'> <meta name='viewport' content='width=device-width, initial-scale=1'> <title>Mail cancelación</title> </head> <body> <table style='max-width: 600px; width:100%;height: 100vh;margin:auto;border-spacing: 0px;'> <thead> <tr style='height:65px;background-color: #373fc2;'> <th><img src='http://todoreservas.com.ar/panel/assets/img/logo.jpg' alt='Todo Reservas'></th> </tr> </thead> <tbody> <tr style='height: 167px;background-color: #454edb;display: block;'> <th style='width: 100%;display: block;'> <span style='font-family: Roboto;font-size: 25px;font-weight: 400;font-style: normal;font-stretch: normal;line-height: 1.2;letter-spacing: normal;text-align: center;color: #ffffff;display: block;padding-bottom:10px;padding-top: 25px;'>¡Lo sentimos!</span> <span style='font-family: Roboto;font-size: 16px;font-weight: 100;font-style: normal;font-stretch: normal;line-height: 1.2;letter-spacing: normal;text-align: center;color: #ffffff;display: block;'>Su reserva fue cancelada</span> </th> <th style='display: block; margin: auto;'> <img src='http://todoreservas.com.ar/panel/assets/img/ticket.png' alt='Ticket' style='padding-top: 20px;'> </th> </tr> <tr style='display: block;border-left: 1px solid #cccccc; border-right: 1px solid #cccccc;padding-bottom: 50px;'> <th style='width: 100%;display: block;padding-top: 115px;'> <span style='font-family: Roboto;font-size: 14px;font-weight: 600;font-style: normal;font-stretch: normal;line-height: 1.14;letter-spacing: normal;text-align: center;color: #060706;display: block;'>Motivo de la cancelación</span> <span style='font-family: Roboto;font-size: 14px; font-weight: 300; font-style: normal; font-stretch: normal; line-height: 1.14;letter-spacing: normal;text-align: center;color: #060706;display: block;padding-top:10px'>" + cancelAppointmentDto.Comment + "</span> <span style='display: block;padding-top: 40px;'><a href='http://todoreservas.com.ar/' style='font-family: Roboto;font-size: 12px;font-weight: 500;font-style: normal;font-stretch: normal;line-height: 30px;letter-spacing: normal;text-align: center;color: #ffffff;height: 30px;border-radius: 15px;background-color: #00b900;display:inline-block;padding: 0px 10px;text-decoration: none;'>REPROGRAMAR RESERVA</a></span> </th> </tr> <tr style='display: block; padding-top: 30px;padding-bottom: 30px;border: 1px solid #ccc;'> <th style='width:100%;text-align:center;display: block;'> <span style='font-family: Roboto;font-size: 12.5px;font-weight: 300;font-style: normal;font-stretch: normal;line-height: 1.17;letter-spacing: normal;text-align: center;color: #060706;padding-right: 10px;'>¿Tiene dudas?</span> <span style='border-radius: 13px;border: 1px solid #030303;padding:3px 10px;'><a href='http://todoreservas.com.ar/preguntasFrecuentes' style='font-family: Roboto;font-size: 11px;font-weight: 300;font-style: normal;font-stretch: normal;line-height: 1.2;letter-spacing: normal;text-align: center;color: #030303;text-decoration: none'>CENTRO DE AYUDA</a></span> </th> </tr> </tbody> </table> </body></html>";

                emailMessage = new EmailDto
                {
                    From = "no-reply@tuturno.com.ar",
                    Subject = $"{clinic.Name} - Cancelacion de turno",
                    To = new List<string> { appointment.Patient.Client.User.Email },
                    Message = template
                };

                appointment.State = AppointmentStateEnum.Cancelled;
                dbContext.SaveChanges();
            }

            emailService.Send(emailMessage);
        }

        /// <summary>
        /// Siendo Clinica, Completa un turno
        /// </summary>
        /// <param name="getAppointmentDto"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public void CompleteAppointmentByClinic([FromBody] IdDto completeAppointmentDto)
        {
            var emailMessage = new EmailDto();

            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

                var appointment = dbContext.Clinic_Appointments.FirstOrDefault(a => a.Id == completeAppointmentDto.Id && a.UserId == userId);
                var clinic = dbContext.Clinics.FirstOrDefault(c => c.UserId == appointment.UserId);

                if (appointment == null)
                {
                    throw new BadRequestException();
                }

                if (appointment.DateTime > DateTime.Now)
                {
                    throw new BadRequestException(ExceptionMessages.AppointmentCantBeCompleted);
                }

                if (clinic == null)
                {
                    throw new BadRequestException();
                }

                string template = "<html lang='en'> <head> <meta charset='UTF-8'> <meta http-equiv='X-UA-Compatible' content='IE=edge'> <meta name='viewport' content='width=device-width, initial-scale=1'> <title>Mail Se Completó el Turno</title> </head> <body> <table style='max-width: 600px; width:100%;height: 100vh;margin:auto;border-spacing: 0px;'> <thead> <tr style='height:65px;background-color: #373fc2;'> <th><img src='http://todoreservas.com.ar/panel/assets/img/logo.jpg' alt='Todo Reservas'></th> </tr> </thead> <tbody> <tr style='height: 167px;background-color: #454edb;display: block;'> <th style='width: 100%;display: block;'> <span style='font-family: Roboto;font-size: 25px;font-weight: 400;font-style: normal;font-stretch: normal;line-height: 1.2;letter-spacing: normal;text-align: center;color: #ffffff;display: block;padding-bottom:10px;padding-top: 50px;'>¡Felicitaciones!</span> <span style='font-family: Roboto;font-size: 16px;font-weight: 100;font-style: normal;font-stretch: normal;line-height: 1.2;letter-spacing: normal;text-align: center;color: #ffffff;display: block;'>Su turno se ha completado</span> </th> <th style='display: block; margin: auto;'> </th> </tr> <tr style='display: block;border-left: 1px solid #cccccc; border-right: 1px solid #cccccc;padding-bottom: 50px;'> <th style='width: 100%;display: block;padding-top: 115px;'> <span style='font-family: Roboto;font-size: 14px; font-weight: 300; font-style: normal; font-stretch: normal; line-height: 1.14;letter-spacing: normal;text-align: center;color: #060706;display: block;padding-top:10px'>Su turno con el doctor " + appointment.Doctor.FirstName + " " + appointment.Doctor.LastName + " del día " + appointment.DateTime.Day + "/" + appointment.DateTime.Month + "/" + appointment.DateTime.Year + " " + appointment.DateTime.Hour + ":" + appointment.DateTime.Minute + " fue completado</span> <span style='display: block;padding-top: 40px;'><a href='http://todoreservas.com.ar/' style='font-family: Roboto;font-size: 12px;font-weight: 500;font-style: normal;font-stretch: normal;line-height: 30px;letter-spacing: normal;text-align: center;color: #ffffff;height: 30px;border-radius: 15px;background-color: #00b900;display:inline-block;padding: 0px 10px;text-decoration: none;'>REALIZAR NUEVA RESERVA</a></span> </th> </tr> <tr style='display: block; padding-top: 30px;padding-bottom: 30px;border: 1px solid #ccc;'> <th style='width:100%;text-align:center;display: block;'> <span style='font-family: Roboto;font-size: 12.5px;font-weight: 300;font-style: normal;font-stretch: normal;line-height: 1.17;letter-spacing: normal;text-align: center;color: #060706;padding-right: 10px;'>¿Tiene dudas?</span> <span style='border-radius: 13px;border: 1px solid #030303;padding:3px 10px;'><a href='http://todoreservas.com.ar/preguntasFrecuentes' style='font-family: Roboto;font-size: 11px;font-weight: 300;font-style: normal;font-stretch: normal;line-height: 1.2;letter-spacing: normal;text-align: center;color: #030303;text-decoration: none'>CENTRO DE AYUDA</a></span> </th> </tr> </tbody> </table> </body></html>";

                appointment.State = AppointmentStateEnum.Completed;

                emailMessage = new EmailDto
                {
                    From = "no-reply@tuturno.com.ar",
                    Subject = "Turno completado",
                    To = new List<string> { appointment.Patient.Client.User.Email },
                    Message = template
                };

                dbContext.SaveChanges();
            }

            emailService.Send(emailMessage);
        }

        /// <summary>
        /// Siendo Cliente, Cancela un turno
        /// </summary>
        /// <param name="getAppointmentDto"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public void CancelAppointment([FromBody] CancelAppointmentDto cancelAppointmentDto)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

                var appointment = dbContext.Clinic_Appointments.FirstOrDefault(a => a.Id == cancelAppointmentDto.Id);

                if (appointment == null)
                {
                    throw new BadRequestException();
                }

                var maxCancelDateTime = appointment.DateTime.AddHours(-24);

                if (DateTime.Now >= maxCancelDateTime)
                {
                    throw new BadRequestException(ExceptionMessages.AppointmentCantBeCanceled);
                }

                appointment.State = AppointmentStateEnum.Cancelled;

                dbContext.SaveChanges();
            }
        }

        /// <summary>
        /// Siendo Cliente, Completa un turno
        /// </summary>
        /// <param name="getAppointmentDto"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public void CompleteAppointment([FromBody] CompleteAppointmentDto completeAppointmentDto)
        {
            var emailMessage = new EmailDto();

            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

                var appointment = dbContext.Clinic_Appointments.FirstOrDefault(a => a.Id == completeAppointmentDto.Id);
                var clinic = dbContext.Clinics.FirstOrDefault(c => c.UserId == appointment.UserId);

                if (appointment == null)
                {
                    throw new BadRequestException();
                }

                if (appointment.DateTime > DateTime.Now)
                {
                    throw new BadRequestException(ExceptionMessages.AppointmentCantBeCompleted);
                }

                if (clinic == null)
                {
                    throw new BadRequestException();
                }

                string template = "<html lang='en'> <head> <meta charset='UTF-8'> <meta http-equiv='X-UA-Compatible' content='IE=edge'> <meta name='viewport' content='width=device-width, initial-scale=1'> <title>Mail Se Completó el Turno</title> </head> <body> <table style='max-width: 600px; width:100%;height: 100vh;margin:auto;border-spacing: 0px;'> <thead> <tr style='height:65px;background-color: #373fc2;'> <th><img src='http://todoreservas.com.ar/panel/assets/img/logo.jpg' alt='Todo Reservas'></th> </tr> </thead> <tbody> <tr style='height: 167px;background-color: #454edb;display: block;'> <th style='width: 100%;display: block;'> <span style='font-family: Roboto;font-size: 25px;font-weight: 400;font-style: normal;font-stretch: normal;line-height: 1.2;letter-spacing: normal;text-align: center;color: #ffffff;display: block;padding-bottom:10px;padding-top: 50px;'>¡Felicitaciones!</span> <span style='font-family: Roboto;font-size: 16px;font-weight: 100;font-style: normal;font-stretch: normal;line-height: 1.2;letter-spacing: normal;text-align: center;color: #ffffff;display: block;'>Se ha completado el turno</span> </th> <th style='display: block; margin: auto;'> </th> </tr> <tr style='display: block;border-left: 1px solid #cccccc; border-right: 1px solid #cccccc;padding-bottom: 50px;'> <th style='width: 100%;display: block;padding-top: 115px;'> <span style='font-family: Roboto;font-size: 14px; font-weight: 300; font-style: normal; font-stretch: normal; line-height: 1.14;letter-spacing: normal;text-align: center;color: #060706;display: block;padding-top:10px'>El turno con el paciente " + appointment.Patient.FullName + " del día " + appointment.DateTime.Day + "/" + appointment.DateTime.Month + "/" + appointment.DateTime.Year + " " + appointment.DateTime.Hour + ":" + appointment.DateTime.Minute + " fue completado con un puntaje de " + completeAppointmentDto.Score + " puntos y el siguiente comentario: <i>" + completeAppointmentDto.Comment + "</i></span> <span style='display: block;padding-top: 40px;'><a href='http://todoreservas.com.ar/panel' style='font-family: Roboto;font-size: 12px;font-weight: 500;font-style: normal;font-stretch: normal;line-height: 30px;letter-spacing: normal;text-align: center;color: #ffffff;height: 30px;border-radius: 15px;background-color: #00b900;display:inline-block;padding: 0px 10px;text-decoration: none;'>VER MIS RESERVAS</a></span> </th> </tr> <tr style='display: block; padding-top: 30px;padding-bottom: 30px;border: 1px solid #ccc;'> <th style='width:100%;text-align:center;display: block;'> <span style='font-family: Roboto;font-size: 12.5px;font-weight: 300;font-style: normal;font-stretch: normal;line-height: 1.17;letter-spacing: normal;text-align: center;color: #060706;padding-right: 10px;'>¿Tiene dudas?</span> <span style='border-radius: 13px;border: 1px solid #030303;padding:3px 10px;'><a href='http://todoreservas.com.ar/preguntasFrecuentes' style='font-family: Roboto;font-size: 11px;font-weight: 300;font-style: normal;font-stretch: normal;line-height: 1.2;letter-spacing: normal;text-align: center;color: #030303;text-decoration: none'>CENTRO DE AYUDA</a></span> </th> </tr> </tbody> </table> </body></html>";

                appointment.State = AppointmentStateEnum.Completed;

                emailMessage = new EmailDto
                {
                    From = "no-reply@tuturno.com.ar",
                    Subject = "Turno completado",
                    To = new List<string> { clinic.User.Email },
                    Message = template
                };

                appointment.Rating = new Clinic_Rating
                {
                    AppointmentId = appointment.Id,
                    Score = completeAppointmentDto.Score,
                    Comment = completeAppointmentDto.Comment,
                    DateTime = DateTime.Now,
                    UserId = clinic.UserId
                };

                dbContext.SaveChanges();
            }

            emailService.Send(emailMessage);
        }

        [HttpPost]
        [Authorize]
        public List<RequestedAppointmentsByDoctorDto> GetRequestedAppointmentsByFilter([FromBody] FilterRequestedAppointmentDto filter)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

                var patients = dbContext.Clinic_Patients.Where(p => p.UserId == userId).ToList();

                var doctors = dbContext.Clinic_Doctors
                    .Where(d => d.UserId == userId)
                    .Where(d => !filter.SpecialtyId.HasValue || d.SpecialtyId == filter.SpecialtyId)
                    .Where(d => !filter.SubspecialtyId.HasValue || d.SubspecialtyId == filter.SubspecialtyId)
                    .ToList();

                return doctors.Select(d => new RequestedAppointmentsByDoctorDto
                {
                    DoctorId = d.Id,
                    DoctorFirstName = d.FirstName,
                    DoctorLastName = d.LastName,
                    RequestedAppointmentsPerHour = Enumerable.Range(0, 24)
                    .Select(hour => new AppointmentsPerHourDto {
                        Hour = hour,
                        Appointments = d.Appointments
                            .Where(a => a.DateTime.Date == filter.Day.Date)
                            .Where(a => a.DateTime.Hour == hour)
                            .Select(a => new AppointmentDto {
                                Id = a.Id,
                                Hour = a.DateTime,
                                Patient = patients.FirstOrDefault(p => p.Id == a.PatientId)?.FullName ?? string.Empty,
                                State = (int) a.State
                            })
                            .OrderBy(a => a.Hour.Minute)
                            .ToList()
                    })
                    .SkipWhile(h => !h.Appointments.Any())
                    .Reverse()
                    .SkipWhile(h => !h.Appointments.Any())
                    .Reverse()
                    .ToList()
                }).ToList();
            }
        }

        /// <summary>
        /// Devuelve entre un rango de fechas, separado en horas con muchos datos internos para poder completar el calendario de una clinica con los turnos que una clinica tiene.
        /// </summary>
        /// <param name="getAppointmentDto"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public List<DayDto> GetWeek([FromBody] FilterWeekAppointmentDto filter)
        {
            // Tengo que devolver una lista con todos los dias entre la fecha desde y la fecha hasta
            // Para cada dia, tengo que partirlo en 24 horas
            // Para cada hora tengo que tener una lista con todas las especialidades del usuario
            // Para cada especialidad tengo que decir cuantos turnos reservados tiene en esa fecha y en ese rango de horas
            var res = new List<DayDto>();

            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

                var specialties = dbContext.Clinic_Specialties.Where(s => s.UserId == userId).ToList();

                var appointments = dbContext.Clinic_Appointments
                    .Where(a => a.Doctor.UserId == userId)
                    .Where(a => !filter.DoctorId.HasValue || a.DoctorId == filter.DoctorId)
                    .Where(a => !filter.SubSpecialtyId.HasValue || a.Doctor.SubspecialtyId == filter.SubSpecialtyId)
                    .Where(a => !filter.SpecialtyId.HasValue || a.Doctor.SpecialtyId == filter.SpecialtyId)
                    .Where(a => filter.StartDate <= a.DateTime && a.DateTime <= filter.EndDate)
                    .ToList();

                for (var date = filter.StartDate.Date; date <= filter.EndDate.Date; date = date.AddDays(1))
                {
                    var day = new DayDto { Day = date, Hours = new List<HourDto>() };
                    var nextDate = date.AddDays(1);

                    for (var datetime = date.AddHours(7); datetime < nextDate; datetime = datetime.AddHours(1))
                    {
                        var hour = new HourDto { Hour = datetime, AppointmentsPerSpecialty = new List<AppointmentsPerSpecialtyDto>(), TotalAppointments = 0 };
                        var nextHour = datetime.AddHours(1);
                        
                        foreach (var specialty in specialties)
                        {
                            var count = appointments.Count(a => datetime <= a.DateTime && a.DateTime < nextHour && a.Doctor.SpecialtyId == specialty.Id);

                            if (count > 0)
                            {
                                hour.AppointmentsPerSpecialty.Add(new AppointmentsPerSpecialtyDto
                                {
                                    SpecialtyId = specialty.Id,
                                    SpecialtyDescription = specialty.Data.Description,
                                    Appointments = count
                                });

                                hour.TotalAppointments += count;
                            }
                        }

                        day.Hours.Add(hour);
                    }

                    res.Add(day);
                }
            }

            return res;
        }

        /// <summary>
        /// Devuelve los turnos de un cliente logueado (solo turnos de clinicas)
        /// </summary>
        /// <param name="getAppointmentDto"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = Roles.Client)]
        public List<ClientDayDto> GetWeekForClient([FromBody] FilterClientWeekAppointmentDto filter)
        {
            var service = new AppointmentService();
            var week = service.Clinic_GetWeekForClient(filter, HttpContext);

            return week;
        }

        /// <summary>
        /// Devuelve los turnos disponibles por dia.
        /// </summary>
        /// <param name="getAppointmentDto"></param>
        /// <returns></returns>
        [HttpPost]
        public List<AppointmentsPerDayDto> GetAvailableAppointmentsPerDay([FromBody] FilterAvailableAppointmentDto filter)
        {
            var res = new List<AppointmentsPerDayDto>();

            using (var dbContext = new ApplicationDbContext())
            {
                var userId = filter.ClinicId.HasValue ? dbContext.Clinics.Where(c => c.Id == filter.ClinicId).Select(c => c.UserId).First() : -1;

                var doctors = dbContext.Clinic_Doctors
                    .Where(d => !filter.ClinicId.HasValue || d.UserId == userId)
                    .Where(d => !filter.DoctorId.HasValue || d.Id == filter.DoctorId)
                    .Where(d => !filter.SubSpecialtyId.HasValue || d.SubspecialtyId == filter.SubSpecialtyId)
                    .Where(d => !filter.SpecialtyId.HasValue || d.SpecialtyId == filter.SpecialtyId)
                    .ToList();

                for (var date = filter.StartDate.Date; date <= filter.EndDate.Date; date = date.AddDays(1))
                {
                    var day = new AppointmentsPerDayDto { Day = date, AvailableAppointments = 0 };

                    foreach (var doctor in doctors)
                    {
                        var availableAppointments = doctor.GetAllAvailablesForDay(date);
                        day.AvailableAppointments += availableAppointments.Count;
                    }

                    res.Add(day);
                }
            }

            return res;
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
