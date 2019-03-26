﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SistemaTurnos.Database;
using SistemaTurnos.Database.HairdressingModel;
using SistemaTurnos.Database.Enums;
using SistemaTurnos.Database.Model;
using SistemaTurnos.WebApplication.WebApi.Dto;
using SistemaTurnos.WebApplication.WebApi.Dto.HairdressingAppointment;
using System;
using System.Collections.Generic;
using System.Linq;
using SistemaTurnos.WebApplication.WebApi.Services;
using SistemaTurnos.Commons.Authorization;
using SistemaTurnos.Commons.Exceptions;
using SistemaTurnos.WebApplication.WebApi.Dto.Email;
using SistemaTurnos.WebApplication.WebApi.Dto.Payment;
using SistemaTurnos.WebApplication.WebApi.Dto.MercadoPago;
using System.Diagnostics;

namespace SistemaTurnos.WebApplication.WebApi.Controllers
{
    [Route("Api/Hairdressing/[controller]/[action]")]
    [Produces("application/json")]
    [EnableCors("AnyOrigin")]
    public class HairdressingAppointmentController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly EmailService _emailService;
        private readonly MercadoPagoService _mercadoPagoService;
        private readonly BusinessPlaceService _businessPlaceService;

        public HairdressingAppointmentController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _emailService = new EmailService();
            _mercadoPagoService = new MercadoPagoService();
            _businessPlaceService = new BusinessPlaceService();
        }

        [HttpPost]
        public List<DateTime> GetAllAvailablesFromDay([FromBody] GetHairdressingAppointmentDto getAppointmentDto)
        {
            var watch = Stopwatch.StartNew();

            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _businessPlaceService.GetUserIdOrDefault(HttpContext);

                if (getAppointmentDto.HairdressingId.HasValue)
                {
                    var hairdressing = dbContext.Hairdressings.FirstOrDefault(h => h.Id == getAppointmentDto.HairdressingId);

                    if (hairdressing == null)
                    {
                        throw new BadRequestException();
                    }

                    userId = hairdressing.UserId;
                }

                if (userId == null)
                {
                    throw new BadRequestException();
                }

                var prof = dbContext.Hairdressing_Professionals.FirstOrDefault(d => d.Id == getAppointmentDto.ProfessionalId && d.UserId == userId);

                if (prof == null)
                {
                    throw new BadRequestException();
                }

                if (!prof.Subspecialties.Any(ds => ds.SubspecialtyId == getAppointmentDto.SubspecialtyId))
                {
                    throw new BadRequestException();
                }

                var res = new List<DateTime>();
                for (int i = 0; i < 15; i++)
                {
                    foreach (var datetime in prof.GetAllAvailableAppointmentsForDay(getAppointmentDto.Day.AddDays(i), getAppointmentDto.SubspecialtyId))
                    {
                        res.Add(datetime);
                    }
                }

                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                Console.WriteLine("HairdressingAppointment/GetAllAvailablesFromDay milisegundos: " + elapsedMs);

                return res;
            }
        }

        [HttpPost]
        public List<DateTime> GetAllAvailablesForDay([FromBody] GetHairdressingAppointmentDto getAppointmentDto)
        {
            var watch = Stopwatch.StartNew();

            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _businessPlaceService.GetUserIdOrDefault(HttpContext);

                if (getAppointmentDto.HairdressingId.HasValue)
                {
                    var hairdressing = dbContext.Hairdressings.FirstOrDefault(h => h.Id == getAppointmentDto.HairdressingId);

                    if (hairdressing == null)
                    {
                        throw new BadRequestException();
                    }

                    userId = hairdressing.UserId;
                }

                if (userId == null)
                {
                    throw new BadRequestException();
                }

                /*if (getAppointmentDto.HairdressingId != null)
                {
                    userId = getAppointmentDto.HairdressingId;

                }*/

                var prof = dbContext.Hairdressing_Professionals.FirstOrDefault(p => p.Id == getAppointmentDto.ProfessionalId && p.UserId == userId);

                if (prof == null)
                {
                    throw new BadRequestException();
                }

                if (!prof.Subspecialties.Any(ds => ds.SubspecialtyId == getAppointmentDto.SubspecialtyId))
                {
                    throw new BadRequestException();
                }

                var res = prof.GetAllAvailableAppointmentsForDay(getAppointmentDto.Day, getAppointmentDto.SubspecialtyId);

                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                Console.WriteLine("HairdressingAppointment/GetAllAvailablesForDay milisegundos: " + elapsedMs);

                return res;
            }
        }

        [HttpPost]
        [Authorize]
        public void RequestAppointmentForNonClient([FromBody] RequestHairdressingAppointmentForNonClientDto requestAppointmentDto)
        {
            var watch = Stopwatch.StartNew();

            var emailMessage = new EmailDto();

            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _businessPlaceService.GetUserId(HttpContext);

                var hairdressing = dbContext.Hairdressings.FirstOrDefault(h => h.UserId == userId);

                if (hairdressing == null)
                {
                    throw new BadRequestException();
                }

                if (requestAppointmentDto.Day.Date < DateTime.Today.Date)
                {
                    throw new BadRequestException(ExceptionMessages.AppointmentCantBeRequested);
                }

                var prof = dbContext.Hairdressing_Professionals.FirstOrDefault(d => d.Id == requestAppointmentDto.ProfessionalId && d.UserId == userId);

                if (prof == null)
                {
                    throw new BadRequestException();
                }

                if (!prof.Subspecialties.Any(ds => ds.SubspecialtyId == requestAppointmentDto.SubspecialtyId))
                {
                    throw new BadRequestException();
                }

                if (!_roleManager.RoleExistsAsync(Roles.Client).Result)
                {
                    throw new ApplicationException(ExceptionMessages.InternalServerError);
                }

                if (dbContext.Clients.Any(c => c.PhoneNumber == requestAppointmentDto.PhoneNumber))
                {
                    throw new ApplicationException(ExceptionMessages.UsernameAlreadyExists);
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
                };

                dbContext.Clients.Add(client);
                dbContext.SaveChanges();

                var patient = new Hairdressing_Patient
                {
                    
                    UserId = userId,
                    ClientId = client.Id,
                };

                dbContext.Hairdressing_Patients.Add(patient);
                dbContext.SaveChanges();

                var availableAppointments = prof.GetAllAvailableAppointmentsForDay(requestAppointmentDto.Day.Date, requestAppointmentDto.SubspecialtyId);

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

                dbContext.Hairdressing_Appointments.Add(new Hairdressing_Appointment
                {
                    ProfessionalId = requestAppointmentDto.ProfessionalId,
                    Professional = prof,
                    DateTime = appointment,
                    State = AppointmentStateEnum.Paid,
                    Source = AppointmentSourceEnum.Panel,
                    PatientId = patient.Id,
                    SubspecialtyId = requestAppointmentDto.SubspecialtyId,
                    UserId = userId
                });

                dbContext.SaveChanges();

                emailMessage = new EmailDto
                {
                    From = "no-reply@todoreservas.com.ar",
                    Subject = "Turno reservado",
                    To = new List<string> { hairdressing.User.Email, requestAppointmentDto.Email },
                    Message = "turno reservado"
                };
            }

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("HairdressingAppointment/RequestAppointmentForNonClient milisegundos: " + elapsedMs);

            _emailService.Send(emailMessage);
        }

        [HttpPost]
        [Authorize(Roles = Roles.AdministratorAndEmployee)]
        public void RequestAppointmentForClient([FromBody] RequestHairdressingAppointmentForClientDto requestAppointmentDto)
        {
            var watch = Stopwatch.StartNew();

            var emailMessage = new EmailDto();

            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _businessPlaceService.GetUserId(HttpContext);

                var hairdressing = dbContext.Hairdressings.FirstOrDefault(h => h.UserId == userId);

                if (hairdressing == null)
                {
                    throw new BadRequestException();
                }

                if (requestAppointmentDto.Day.Date < DateTime.Today.Date)
                {
                    throw new BadRequestException(ExceptionMessages.AppointmentCantBeRequested);
                }

                var prof = dbContext.Hairdressing_Professionals.FirstOrDefault(d => d.Id == requestAppointmentDto.ProfessionalId && d.UserId == userId);

                if (prof == null)
                {
                    throw new BadRequestException();
                }

                if (!prof.Subspecialties.Any(ds => ds.SubspecialtyId == requestAppointmentDto.SubspecialtyId))
                {
                    throw new BadRequestException();

                }
                var client = dbContext.Clients.FirstOrDefault(c => c.Id == requestAppointmentDto.ClientId);

                if (client == null)
                {
                    throw new BadRequestException();
                }

                var patient = dbContext.Hairdressing_Patients.FirstOrDefault(p => p.ClientId == client.Id && p.UserId == userId);

                if (patient != null)
                {
                    throw new BadRequestException();
                }

                patient = new Hairdressing_Patient
                {
                    UserId = userId,
                    ClientId = requestAppointmentDto.ClientId
                };

                dbContext.Hairdressing_Patients.Add(patient);

                var availableAppointments = prof.GetAllAvailableAppointmentsForDay(requestAppointmentDto.Day.Date, requestAppointmentDto.SubspecialtyId);

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

                dbContext.Hairdressing_Appointments.Add(new Hairdressing_Appointment
                {
                    ProfessionalId = requestAppointmentDto.ProfessionalId,
                    Professional = prof,
                    DateTime = appointment,
                    State = AppointmentStateEnum.Paid,
                    Source = AppointmentSourceEnum.Panel,
                    PatientId = patient.Id,
                    SubspecialtyId = requestAppointmentDto.SubspecialtyId,
                    UserId = userId
                });

                dbContext.SaveChanges();

                emailMessage = new EmailDto
                {
                    From = "no-reply@todoreservas.com.ar",
                    Subject = "Turno reservado",
                    To = new List<string> { hairdressing.User.Email, patient.Client.User.Email },
                    Message = "turno reservado"
                };
            }

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("HairdressingAppointment/RequestAppointmentForClient milisegundos: " + elapsedMs);

            _emailService.Send(emailMessage);
        }

        [HttpPost]
        [Authorize(Roles = Roles.AdministratorAndEmployee)]
        public void RequestAppointmentForPatient([FromBody] RequestHairdressingAppointmentForPatientDto requestAppointmentDto)
        {
            var watch = Stopwatch.StartNew();

            var emailMessage = new EmailDto();

            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _businessPlaceService.GetUserId(HttpContext);

                var hairdressing = dbContext.Hairdressings.FirstOrDefault(h => h.UserId == userId);

                if (hairdressing == null)
                {
                    throw new BadRequestException();
                }

                if (requestAppointmentDto.Day.Date < DateTime.Today.Date)
                {
                    throw new BadRequestException(ExceptionMessages.AppointmentCantBeRequested);
                }

                var prof = dbContext.Hairdressing_Professionals.FirstOrDefault(d => d.Id == requestAppointmentDto.ProfessionalId && d.UserId == userId);

                if (prof == null)
                {
                    throw new BadRequestException();
                }

                if (!prof.Subspecialties.Any(ds => ds.SubspecialtyId == requestAppointmentDto.SubspecialtyId))
                {
                    throw new BadRequestException();
                }

                var patient = dbContext.Hairdressing_Patients.FirstOrDefault(p => p.ClientId == requestAppointmentDto.PatientId);

                if (patient == null)
                {
                    throw new BadRequestException();
                }

                var availableAppointments = prof.GetAllAvailableAppointmentsForDay(requestAppointmentDto.Day.Date, requestAppointmentDto.SubspecialtyId);

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

                dbContext.Hairdressing_Appointments.Add(new Hairdressing_Appointment
                {
                    ProfessionalId = requestAppointmentDto.ProfessionalId,
                    Professional = prof,
                    DateTime = appointment,
                    State = AppointmentStateEnum.Paid,
                    Source = AppointmentSourceEnum.Panel,
                    PatientId = patient.Id,
                    SubspecialtyId = requestAppointmentDto.SubspecialtyId,
                    UserId = userId
                });

                dbContext.SaveChanges();

                OneSignalService.ScheduleNotifications(hairdressing.UserId, appointment);

                emailMessage = new EmailDto
                {
                    From = "no-reply@todoreservas.com.ar",
                    Subject = "Turno reservado",
                    To = new List<string> { hairdressing.User.Email, patient.Client.User.Email },
                    Message = "turno reservado"
                };
            }

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("HairdressingAppointment/RequestAppointmentForPatient milisegundos: " + elapsedMs);

            _emailService.Send(emailMessage);
        }

        [HttpPost]
        [Authorize(Roles = Roles.Client)]
        public PaymentDto RequestAppointmentByClient([FromBody] RequestHairdressingAppointmentByClientDto requestAppointmentDto)
        {
            var watch = Stopwatch.StartNew();

            var emailMessage = new EmailDto();

            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _businessPlaceService.GetUserId(HttpContext);

                if (requestAppointmentDto.Day.Date < DateTime.Today.Date)
                {
                    throw new BadRequestException(ExceptionMessages.AppointmentCantBeRequested);
                }

                var hairdressing = dbContext.Hairdressings.FirstOrDefault(c => c.Id == requestAppointmentDto.HairdressingId);

                if (hairdressing == null)
                {
                    throw new BadRequestException();
                }

                var prof = dbContext.Hairdressing_Professionals.FirstOrDefault(d => d.Id == requestAppointmentDto.ProfessionalId && d.UserId == hairdressing.UserId);

                if (prof == null)
                {
                    throw new BadRequestException();
                }

                if (!prof.Subspecialties.Any(ds => ds.SubspecialtyId == requestAppointmentDto.SubspecialtyId))
                {
                    throw new BadRequestException();
                }

                var client = dbContext.Clients.FirstOrDefault(c => c.UserId == userId);

                if (client == null)
                {
                    throw new BadRequestException();
                }

                var patient = dbContext.Hairdressing_Patients.FirstOrDefault(p => p.ClientId == client.Id && p.UserId == hairdressing.UserId);

                if (patient != null)
                {
                    throw new BadRequestException();
                }

                patient = new Hairdressing_Patient
                {
                    UserId = hairdressing.UserId,
                    ClientId = client.Id,
                };

                dbContext.Hairdressing_Patients.Add(patient);

                var availableAppointments = prof.GetAllAvailableAppointmentsForDay(requestAppointmentDto.Day.Date, requestAppointmentDto.SubspecialtyId);

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

                var hairdressingAppointment = new Hairdressing_Appointment
                {
                    ProfessionalId = requestAppointmentDto.ProfessionalId,
                    Professional = prof,
                    DateTime = appointment,
                    State = AppointmentStateEnum.Reserved,
                    Source = (AppointmentSourceEnum) requestAppointmentDto.Source,
                    PatientId = patient.Id,
                    SubspecialtyId = requestAppointmentDto.SubspecialtyId,
                    UserId = hairdressing.UserId
                };

                dbContext.Hairdressing_Appointments.Add(hairdressingAppointment);

                dbContext.SaveChanges();

                var paymentInformation = GeneratePaymentLink(hairdressing, hairdressingAppointment, client.User.Email);

                if (paymentInformation == null)
                {
                    throw new ApplicationException(ExceptionMessages.InternalServerError);
                }

                hairdressingAppointment.PreferenceId = paymentInformation.PreferenceId;
                dbContext.SaveChanges();

                OneSignalService.ScheduleNotifications(hairdressing.UserId, appointment);

                emailMessage = new EmailDto
                {
                    From = "no-reply@todoreservas.com.ar",
                    Subject = "Turno reservado",
                    To = new List<string> { hairdressing.User.Email, patient.Client.User.Email },
                    Message = "turno reservado"
                };

                _emailService.Send(emailMessage);

                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                Console.WriteLine("HairdressingAppointment/RequestAppointmentByClient milisegundos: " + elapsedMs);

                return new PaymentDto
                {
                    PaymentLink = paymentInformation.PaymentLink
                };
            }
        }

        [HttpPost]
        [Authorize(Roles = Roles.Client)]
        public PaymentDto RequestAppointmentByPatient([FromBody] RequestHairdressingAppointmentByPatientDto requestAppointmentDto)
        {
            var watch = Stopwatch.StartNew();

            var emailMessage = new EmailDto();

            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _businessPlaceService.GetUserId(HttpContext);

                if (requestAppointmentDto.Day.Date < DateTime.Today.Date)
                {
                    throw new BadRequestException(ExceptionMessages.AppointmentCantBeRequested);
                }

                var hairdressing = dbContext.Hairdressings.FirstOrDefault(c => c.Id == requestAppointmentDto.HairdressingId);

                if (hairdressing == null)
                {
                    throw new BadRequestException();
                }

                var prof = dbContext.Hairdressing_Professionals.FirstOrDefault(d => d.Id == requestAppointmentDto.ProfessionalId && d.UserId == hairdressing.UserId);

                if (prof == null)
                {
                    throw new BadRequestException();
                }

                if (!prof.Subspecialties.Any(ds => ds.SubspecialtyId == requestAppointmentDto.SubspecialtyId))
                {
                    throw new BadRequestException();
                }

                var client = dbContext.Clients.FirstOrDefault(c => c.UserId == userId);
                var patient = dbContext.Hairdressing_Patients.FirstOrDefault(p => p.ClientId == client.Id && p.UserId == hairdressing.UserId);

                if (patient == null)
                {
                    throw new BadRequestException();
                }

                var availableAppointments = prof.GetAllAvailableAppointmentsForDay(requestAppointmentDto.Day.Date, requestAppointmentDto.SubspecialtyId);

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

                var hairdressingAppointment = new Hairdressing_Appointment
                {
                    ProfessionalId = requestAppointmentDto.ProfessionalId,
                    Professional = prof,
                    DateTime = appointment,
                    State = AppointmentStateEnum.Reserved,
                    Source = (AppointmentSourceEnum)requestAppointmentDto.Source,
                    PatientId = patient.Id,
                    SubspecialtyId = requestAppointmentDto.SubspecialtyId,
                    UserId = hairdressing.UserId
                };

                dbContext.Hairdressing_Appointments.Add(hairdressingAppointment);

                dbContext.SaveChanges();

                var paymentInformation = GeneratePaymentLink(hairdressing, hairdressingAppointment, client.User.Email);

                if (paymentInformation == null)
                {
                    throw new ApplicationException(ExceptionMessages.InternalServerError);
                }

                hairdressingAppointment.PreferenceId = paymentInformation.PreferenceId;
                dbContext.SaveChanges();

                emailMessage = new EmailDto
                {
                    From = "no-reply@todoreservas.com.ar",
                    Subject = "Turno reservado",
                    To = new List<string> { hairdressing.User.Email, patient.Client.User.Email },
                    Message = "turno reservado"
                };

                _emailService.Send(emailMessage);

                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                Console.WriteLine("HairdressingAppointment/RequestAppointmentByPatient milisegundos: " + elapsedMs);

                return new PaymentDto
                {
                    PaymentLink = paymentInformation.PaymentLink
                };
            }
        }

        [HttpPost]
        [Authorize(Roles = Roles.AdministratorAndEmployee)]
        public void CancelAppointmentByHairdressing([FromBody] CancelHairdressingAppointmentDto cancelAppointmentDto)
        {
            var watch = Stopwatch.StartNew();

            var emailMessage = new EmailDto();

            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _businessPlaceService.GetUserId(HttpContext);

                var appointment = dbContext.Hairdressing_Appointments.FirstOrDefault(a => a.Id == cancelAppointmentDto.Id && a.UserId == userId);

                if (appointment == null)
                {
                    throw new BadRequestException();
                }

                var hairdressing = dbContext.Hairdressings.FirstOrDefault(c => c.UserId == userId);

                if (hairdressing == null)
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
                    From = "no-reply@todoreservas.com.ar",
                    Subject = $"{hairdressing.Name} - Cancelacion de turno",
                    To = new List<string> { appointment.Patient.Client.User.Email },
                    Message = template
                };

                appointment.State = AppointmentStateEnum.Cancelled;
                dbContext.SaveChanges();
            }

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("HairdressingAppointment/CancelAppointmentByHairdressing milisegundos: " + elapsedMs);

            _emailService.Send(emailMessage);
        }

        [HttpPost]
        [Authorize]
        public void CompleteAppointmentByHairdressing([FromBody] IdDto completeAppointmentDto)
        {
            var watch = Stopwatch.StartNew();

            var emailMessage = new EmailDto();

            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _businessPlaceService.GetUserId(HttpContext);

                var appointment = dbContext.Hairdressing_Appointments.FirstOrDefault(a => a.Id == completeAppointmentDto.Id && a.UserId == userId);
                var hairdressing = dbContext.Hairdressings.FirstOrDefault(c => c.UserId == userId);

                if (appointment == null)
                {
                    throw new BadRequestException();
                }

                if (appointment.DateTime > DateTime.Now)
                {
                    throw new BadRequestException(ExceptionMessages.AppointmentCantBeCompleted);
                }

                if (hairdressing == null)
                {
                    throw new BadRequestException();
                }

                string template = "<html lang='en'> <head> <meta charset='UTF-8'> <meta http-equiv='X-UA-Compatible' content='IE=edge'> <meta name='viewport' content='width=device-width, initial-scale=1'> <title>Mail Se Completó el Turno</title> </head> <body> <table style='max-width: 600px; width:100%;height: 100vh;margin:auto;border-spacing: 0px;'> <thead> <tr style='height:65px;background-color: #373fc2;'> <th><img src='http://todoreservas.com.ar/panel/assets/img/logo.jpg' alt='Todo Reservas'></th> </tr> </thead> <tbody> <tr style='height: 167px;background-color: #454edb;display: block;'> <th style='width: 100%;display: block;'> <span style='font-family: Roboto;font-size: 25px;font-weight: 400;font-style: normal;font-stretch: normal;line-height: 1.2;letter-spacing: normal;text-align: center;color: #ffffff;display: block;padding-bottom:10px;padding-top: 50px;'>¡Felicitaciones!</span> <span style='font-family: Roboto;font-size: 16px;font-weight: 100;font-style: normal;font-stretch: normal;line-height: 1.2;letter-spacing: normal;text-align: center;color: #ffffff;display: block;'>Su turno se ha completado</span> </th> <th style='display: block; margin: auto;'> </th> </tr> <tr style='display: block;border-left: 1px solid #cccccc; border-right: 1px solid #cccccc;padding-bottom: 50px;'> <th style='width: 100%;display: block;padding-top: 115px;'> <span style='font-family: Roboto;font-size: 14px; font-weight: 300; font-style: normal; font-stretch: normal; line-height: 1.14;letter-spacing: normal;text-align: center;color: #060706;display: block;padding-top:10px'>Su turno con el profesional " + appointment.Professional.FirstName + " " + appointment.Professional.LastName + " del día " + appointment.DateTime.Day + "/" + appointment.DateTime.Month + "/" + appointment.DateTime.Year + " " + appointment.DateTime.Hour + ":" + appointment.DateTime.Minute + " fue completado</span> <span style='display: block;padding-top: 40px;'><a href='http://todoreservas.com.ar/' style='font-family: Roboto;font-size: 12px;font-weight: 500;font-style: normal;font-stretch: normal;line-height: 30px;letter-spacing: normal;text-align: center;color: #ffffff;height: 30px;border-radius: 15px;background-color: #00b900;display:inline-block;padding: 0px 10px;text-decoration: none;'>REALIZAR NUEVA RESERVA</a></span> </th> </tr> <tr style='display: block; padding-top: 30px;padding-bottom: 30px;border: 1px solid #ccc;'> <th style='width:100%;text-align:center;display: block;'> <span style='font-family: Roboto;font-size: 12.5px;font-weight: 300;font-style: normal;font-stretch: normal;line-height: 1.17;letter-spacing: normal;text-align: center;color: #060706;padding-right: 10px;'>¿Tiene dudas?</span> <span style='border-radius: 13px;border: 1px solid #030303;padding:3px 10px;'><a href='http://todoreservas.com.ar/preguntasFrecuentes' style='font-family: Roboto;font-size: 11px;font-weight: 300;font-style: normal;font-stretch: normal;line-height: 1.2;letter-spacing: normal;text-align: center;color: #030303;text-decoration: none'>CENTRO DE AYUDA</a></span> </th> </tr> </tbody> </table> </body></html>";

                appointment.State = AppointmentStateEnum.Completed;

                emailMessage = new EmailDto
                {
                    From = "no-reply@todoreservas.com.ar",
                    Subject = "Turno completado",
                    To = new List<string> { appointment.Patient.Client.User.Email },
                    Message = template
                };

                dbContext.SaveChanges();
            }

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("HairdressingAppointment/CompleteAppointmentByHairdressing milisegundos: " + elapsedMs);

            _emailService.Send(emailMessage);
        }

        [HttpPost]
        [Authorize]
        public void CancelAppointment([FromBody] CancelHairdressingAppointmentDto cancelAppointmentDto)
        {
            var watch = Stopwatch.StartNew();

            var emailMessage = new EmailDto();

            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _businessPlaceService.GetUserId(HttpContext);

                var appointment = dbContext.Hairdressing_Appointments.FirstOrDefault(a => a.Id == cancelAppointmentDto.Id && a.State != AppointmentStateEnum.Completed);

                if (appointment == null)
                {
                    throw new BadRequestException();
                }

                var hairdressing = dbContext.Hairdressings.FirstOrDefault(h => h.UserId == appointment.UserId);

                if (hairdressing == null)
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

                emailMessage = new EmailDto
                {
                    From = "no-reply@todoreservas.com.ar",
                    Subject = "Turno cancelado",
                    To = new List<string> { hairdressing.User.Email, appointment.Patient.Client.User.Email },
                    Message = "Turno cancelado"
                };
            }

            _emailService.Send(emailMessage);

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("HairdressingAppointment/CancelAppointment milisegundos: " + elapsedMs);
        }

        [HttpPost]
        [Authorize]
        public void CompleteAppointment([FromBody] CompleteHairdressingAppointmentDto completeAppointmentDto)
        {
            var watch = Stopwatch.StartNew();

            var emailMessage = new EmailDto();

            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _businessPlaceService.GetUserId(HttpContext);

                var appointment = dbContext.Hairdressing_Appointments.FirstOrDefault(a => a.Id == completeAppointmentDto.Id);
                var hairdressing = dbContext.Hairdressings.FirstOrDefault(c => c.UserId == appointment.Professional.UserId);

                if (appointment == null)
                {
                    throw new BadRequestException();
                }

                var appointmentRequiredState = hairdressing.RequiresPayment ? AppointmentStateEnum.Paid : AppointmentStateEnum.Reserved;

                if  (appointment.State != appointmentRequiredState)
                {
                    throw new BadRequestException();
                }

                if (appointment.DateTime > DateTime.Now)
                {
                    throw new BadRequestException(ExceptionMessages.AppointmentCantBeCompleted);
                }

                if (hairdressing == null)
                {
                    throw new BadRequestException();
                }

                string template = "<html lang='en'> <head> <meta charset='UTF-8'> <meta http-equiv='X-UA-Compatible' content='IE=edge'> <meta name='viewport' content='width=device-width, initial-scale=1'> <title>Mail Se Completó el Turno</title> </head> <body> <table style='max-width: 600px; width:100%;height: 100vh;margin:auto;border-spacing: 0px;'> <thead> <tr style='height:65px;background-color: #373fc2;'> <th><img src='http://todoreservas.com.ar/panel/assets/img/logo.jpg' alt='Todo Reservas'></th> </tr> </thead> <tbody> <tr style='height: 167px;background-color: #454edb;display: block;'> <th style='width: 100%;display: block;'> <span style='font-family: Roboto;font-size: 25px;font-weight: 400;font-style: normal;font-stretch: normal;line-height: 1.2;letter-spacing: normal;text-align: center;color: #ffffff;display: block;padding-bottom:10px;padding-top: 50px;'>¡Felicitaciones!</span> <span style='font-family: Roboto;font-size: 16px;font-weight: 100;font-style: normal;font-stretch: normal;line-height: 1.2;letter-spacing: normal;text-align: center;color: #ffffff;display: block;'>Se ha completado el turno</span> </th> <th style='display: block; margin: auto;'> </th> </tr> <tr style='display: block;border-left: 1px solid #cccccc; border-right: 1px solid #cccccc;padding-bottom: 50px;'> <th style='width: 100%;display: block;padding-top: 115px;'> <span style='font-family: Roboto;font-size: 14px; font-weight: 300; font-style: normal; font-stretch: normal; line-height: 1.14;letter-spacing: normal;text-align: center;color: #060706;display: block;padding-top:10px'>El turno con el paciente " + appointment.Patient.Client.FullName + " del día " + appointment.DateTime.Day + "/" + appointment.DateTime.Month + "/" + appointment.DateTime.Year + " " + appointment.DateTime.Hour + ":" + appointment.DateTime.Minute + " fue completado con un puntaje de " + completeAppointmentDto.Score + " puntos y el siguiente comentario: <i>" + completeAppointmentDto.Comment + "</i></span> <span style='display: block;padding-top: 40px;'><a href='http://todoreservas.com.ar/panel' style='font-family: Roboto;font-size: 12px;font-weight: 500;font-style: normal;font-stretch: normal;line-height: 30px;letter-spacing: normal;text-align: center;color: #ffffff;height: 30px;border-radius: 15px;background-color: #00b900;display:inline-block;padding: 0px 10px;text-decoration: none;'>VER MIS RESERVAS</a></span> </th> </tr> <tr style='display: block; padding-top: 30px;padding-bottom: 30px;border: 1px solid #ccc;'> <th style='width:100%;text-align:center;display: block;'> <span style='font-family: Roboto;font-size: 12.5px;font-weight: 300;font-style: normal;font-stretch: normal;line-height: 1.17;letter-spacing: normal;text-align: center;color: #060706;padding-right: 10px;'>¿Tiene dudas?</span> <span style='border-radius: 13px;border: 1px solid #030303;padding:3px 10px;'><a href='http://todoreservas.com.ar/preguntasFrecuentes' style='font-family: Roboto;font-size: 11px;font-weight: 300;font-style: normal;font-stretch: normal;line-height: 1.2;letter-spacing: normal;text-align: center;color: #030303;text-decoration: none'>CENTRO DE AYUDA</a></span> </th> </tr> </tbody> </table> </body></html>";

                appointment.State = AppointmentStateEnum.Completed;

                emailMessage = new EmailDto
                {
                    From = "no-reply@todoreservas.com.ar",
                    Subject = "Turno completado",
                    To = new List<string> { appointment.Patient.Client.User.Email },
                    Message = template
                };

                appointment.Rating = new Hairdressing_Rating
                {
                    AppointmentId = appointment.Id,
                    Score = completeAppointmentDto.Score,
                    Comment = completeAppointmentDto.Comment,
                    DateTime = DateTime.Now,
                    UserId = userId
                };

                dbContext.SaveChanges();
            }

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("HairdressingAppointment/CompleteAppointment milisegundos: " + elapsedMs);

            _emailService.Send(emailMessage);
        }

        [HttpPost]
        [Authorize]
        public List<RequestedHairdressingAppointmentsByProfessionalDto> GetRequestedAppointmentsByFilter([FromBody] FilterRequestedHairdressingAppointmentDto filter)
        {
            var watch = Stopwatch.StartNew();

            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _businessPlaceService.GetUserId(HttpContext);

                var patients = dbContext.Hairdressing_Patients.Where(p => p.UserId == userId).ToList();

                var profs = dbContext.Hairdressing_Professionals
                    .Where(d => d.UserId == userId)
                    .Where(d => !filter.SpecialtyId.HasValue || d.Subspecialties.Any(ssp => ssp.Subspecialty.SpecialtyId == filter.SpecialtyId))
                    .Where(d => !filter.SubspecialtyId.HasValue || d.Subspecialties.Any(ssp => ssp.SubspecialtyId == filter.SubspecialtyId))
                    .ToList();

                var res = profs.Select(d => new RequestedHairdressingAppointmentsByProfessionalDto
                {
                    ProfessionalId = d.Id,
                    ProfessionalFirstName = d.FirstName,
                    ProfessionalLastName = d.LastName,
                    RequestedAppointmentsPerHour = Enumerable.Range(0, 24)
                    .Select(hour => new HairdressingAppointmentsPerHourDto
                    {
                        Hour = hour,
                        Appointments = d.Appointments
                            .Where(a => a.DateTime.Date == filter.Day.Date)
                            .Where(a => a.DateTime.Hour == hour)
                            .Select(a => new HairdressingAppointmentDto {
                                Id = a.Id,
                                Hour = a.DateTime,
                                Patient = patients.FirstOrDefault(p => p.Id == a.PatientId)?.Client?.FullName ?? string.Empty,
                                State = (int) a.State,
                                Specialty = a.Subspecialty.Specialty.Data.Description,
                                Subspecialty = a.Subspecialty.Data.Description
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

                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                Console.WriteLine("HairdressingAppointment/GetRequestedAppointmentsByFilter milisegundos: " + elapsedMs);

                return res;
            }
        }

        [HttpPost]
        [Authorize]
        public List<DayDto> GetWeek([FromBody] FilterWeekHairdressingAppointmentDto filter)
        {
            var watch = Stopwatch.StartNew();

            // Tengo que devolver una lista con todos los dias entre la fecha desde y la fecha hasta
            // Para cada dia, tengo que partirlo en 24 horas
            // Para cada hora tengo que tener una lista con todas las especialidades del usuario
            // Para cada especialidad tengo que decir cuantos turnos reservados tiene en esa fecha y en ese rango de horas
            var res = new List<DayDto>();

            using (var dbContext = new ApplicationDbContext())
            {
                var userId = _businessPlaceService.GetUserId(HttpContext);

                var specialties = dbContext.Hairdressing_Specialties.Where(s => s.UserId == userId).ToList();

                var appointments = dbContext.Hairdressing_Appointments
                    .Where(a => a.Professional.UserId == userId)
                    .Where(a => !filter.ProfessionalId.HasValue || a.ProfessionalId == filter.ProfessionalId)
                    .Where(a => !filter.SubSpecialtyId.HasValue || a.Professional.Subspecialties.Any(ssp => ssp.SubspecialtyId == filter.SubSpecialtyId))
                    .Where(a => !filter.SpecialtyId.HasValue || a.Professional.Subspecialties.Any(ssp => ssp.Subspecialty.SpecialtyId == filter.SpecialtyId))
                    .Where(a => filter.StartDate <= a.DateTime && a.DateTime <= filter.EndDate)
                    .ToList();

                for (var date = filter.StartDate.Date; date <= filter.EndDate.Date; date = date.AddDays(1))
                {
                    var day = new DayDto { Day = date, Hours = new List<HourDto>() };
                    var nextDate = date.AddDays(1);

                    for (var datetime = date.AddHours(7); datetime < nextDate; datetime = datetime.AddHours(1))
                    {
                        var hour = new HourDto { Hour = datetime, AppointmentsPerSpecialty = new List<HairdressingAppointmentsPerSpecialtyDto>(), TotalAppointments = 0 };
                        var nextHour = datetime.AddHours(1);
                        
                        foreach (var specialty in specialties)
                        {
                            var count = appointments.Count(a => datetime <= a.DateTime && a.DateTime < nextHour && a.Subspecialty.SpecialtyId == specialty.Id);

                            if (count > 0)
                            {
                                hour.AppointmentsPerSpecialty.Add(new HairdressingAppointmentsPerSpecialtyDto
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

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("HairdressingAppointment/GetWeek milisegundos: " + elapsedMs);

            return res;
        }

        [HttpPost]
        [Authorize(Roles = Roles.Client)]
        public List<HairdressingClientDayDto> GetWeekForClient([FromBody] FilterClientWeekHairdressingAppointmentDto filter)
        {
            var watch = Stopwatch.StartNew();

            var service = new AppointmentService();
            var week = service.Hairdressing_GetWeekForClient(filter, HttpContext);

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("HairdressingAppointment/GetWeekForClient milisegundos: " + elapsedMs);

            return week;
        }

        [HttpPost]
        public List<HairdressingAppointmentsPerDayDto> GetAvailableAppointmentsPerDay([FromBody] FilterAvailableHairdressingAppointmentDto filter)
        {
            var watch = Stopwatch.StartNew();
            var res = new List<HairdressingAppointmentsPerDayDto>();

            using (var dbContext = new ApplicationDbContext())
            {
                var userId = filter.HairdressingId.HasValue ? dbContext.Hairdressings.Where(c => c.Id == filter.HairdressingId).Select(c => c.UserId).First() : -1;

                var profs = dbContext.Hairdressing_Professionals
                    .Where(d => !filter.ProfessionalId.HasValue || d.UserId == userId)
                    .Where(d => !filter.ProfessionalId.HasValue || d.Id == filter.ProfessionalId)
                    .Where(d => d.Subspecialties.Any(ssp => ssp.SubspecialtyId == filter.SubSpecialtyId))
                    .ToList();

                for (var date = filter.StartDate.Date; date <= filter.EndDate.Date; date = date.AddDays(1))
                {
                    var day = new HairdressingAppointmentsPerDayDto { Day = date, AvailableAppointments = 0 };

                    foreach (var prof in profs)
                    {
                        var availableAppointments = prof.GetAllAvailableAppointmentsForDay(date, filter.SubSpecialtyId);
                        day.AvailableAppointments += availableAppointments.Count;
                    }

                    res.Add(day);
                }
            }

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("HairdressingAppointment/GetWeekForClient milisegundos: " + elapsedMs);

            return res;
        }

        [HttpPost("{appointmentId:int}")]
        public void UpdatePaymentInformation(int appointmentId, [FromQuery(Name = "topic")] string topic, [FromQuery(Name = "id")] string id)
        {
            Console.WriteLine($"Received payment notification. Appointment Id: {appointmentId}, Topic: {topic}, Merchant order Id: {id}");

            if (topic == "merchant_order")
            {
                using (var dbContext = new ApplicationDbContext())
                {
                    var appointment = dbContext.Hairdressing_Appointments.FirstOrDefault(a => a.Id == appointmentId);

                    if (appointment == null)
                    {
                        throw new BadRequestException();
                    }

                    Console.WriteLine($"Se pudo obtener el turno con id: {appointmentId}");

                    var hairdressing = dbContext.Hairdressings.FirstOrDefault(h => h.UserId == appointment.UserId);

                    if (hairdressing == null)
                    {
                        throw new BadRequestException();
                    }

                    Console.WriteLine($"Se pudo obtener la peluqueria con id: {hairdressing.Id}");

                    var merchantOrder = _mercadoPagoService.GetMerchantOrder(new MpGetMerchantOrderRequestDto
                    {
                        ClientId = hairdressing.ClientId,
                        ClientSecret = hairdressing.ClientSecret,
                        MerchantOrderId = id
                    });

                    Console.WriteLine($"Se pudo obtener la merchant order con id: {id}");

                    appointment.MerchantOrder = id;
                    dbContext.SaveChanges();
                }
            }

            if (topic == "payment")
            {
                using (var dbContext = new ApplicationDbContext())
                {
                    var appointment = dbContext.Hairdressing_Appointments.FirstOrDefault(a => a.Id == appointmentId);

                    if (appointment == null)
                    {
                        throw new BadRequestException();
                    }

                    Console.WriteLine($"Se pudo obtener el turno con id: {appointmentId}");

                    var hairdressing = dbContext.Hairdressings.FirstOrDefault(h => h.UserId == appointment.UserId);

                    if (hairdressing == null)
                    {
                        throw new BadRequestException();
                    }

                    Console.WriteLine($"Se pudo obtener la peluqueria con id: {hairdressing.Id}");

                    var merchantOrder = _mercadoPagoService.GetMerchantOrder(new MpGetMerchantOrderRequestDto
                    {
                        ClientId = hairdressing.ClientId,
                        ClientSecret = hairdressing.ClientSecret,
                        MerchantOrderId = appointment.MerchantOrder
                    });

                    Console.WriteLine($"Se pudo obtener la merchant order con id: {id}");

                    if (merchantOrder == null || string.IsNullOrWhiteSpace(merchantOrder.preference_id))
                    {
                        throw new BadRequestException();
                    }

                    Console.WriteLine($"Pagado: {merchantOrder.paid_amount}. Total {merchantOrder.total_amount}");

                    if (merchantOrder.paid_amount != merchantOrder.total_amount)
                    {
                        return;
                    }

                    Console.WriteLine($"Se pago el turno con merchant order {appointment.MerchantOrder} y preference {appointment.PreferenceId}");

                    appointment.State = AppointmentStateEnum.Paid;
                    dbContext.SaveChanges();
                }
            }
        }

        private MpPaymentInformationDto GeneratePaymentLink(Hairdressing hairdressing, Hairdressing_Appointment hairdressingAppointment, string userEmail)
        {
            if (!hairdressing.RequiresPayment)
            {
                return new MpPaymentInformationDto
                {
                    PaymentLink = string.Empty,
                    PreferenceId = string.Empty
                };
            }

            return _mercadoPagoService.GeneratePaymentLink(new MpGeneratePaymentRequestDto
            {
                Id = hairdressingAppointment.Id,
                ClientId = hairdressing.ClientId,
                ClientSecret = hairdressing.ClientSecret,
                Title = $"Turno en peluqueria '{hairdressing.Name}' el dia {hairdressingAppointment.DateTime.ToShortDateString()} en el horario {hairdressingAppointment.DateTime.ToShortTimeString()}",
                Price = hairdressingAppointment?.Subspecialty?.Price ?? 1,
                BuyerEmail = userEmail,
            });
        }
    }
}
