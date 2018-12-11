using Microsoft.AspNetCore.Authorization;
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
using SistemaTurnos.Database.ClinicModel;
using SistemaTurnos.Commons.Authorization;
using SistemaTurnos.Commons.Exceptions;
using SistemaTurnos.WebApplication.WebApi.Dto.Email;
using SistemaTurnos.WebApplication.WebApi.Dto.Payment;
using SistemaTurnos.WebApplication.WebApi.Dto.MercadoPago;

namespace SistemaTurnos.WebApplication.WebApi.Controllers
{
    [Route("Api/Hairdressing/[controller]/[action]")]
    [Produces("application/json")]
    [EnableCors("AnyOrigin")]
    public class HairdressingAppointmentController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly EmailService emailService = new EmailService();
        private readonly MercadoPagoService mercadoPagoService = new MercadoPagoService();

        public HairdressingAppointmentController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpPost]
        [Authorize]
        public List<DateTime> GetAllAvailablesFromDay([FromBody] GetHairdressingAppointmentDto getAppointmentDto)
        {
            using (var dbContext = new ApplicationDbContext())
            {

                var userId = GetUserId();

                var prof = dbContext.Hairdressing_Professionals.FirstOrDefault(d => d.Id == getAppointmentDto.ProfessionalId && d.UserId == userId);

                if (prof == null)
                {
                    throw new BadRequestException();
                }

                var res = new List<DateTime>();
                for (int i = 0; i < 15; i++)
                {
                    foreach (var datetime in prof.GetAllAvailablesForDay(getAppointmentDto.Day.AddDays(i)))
                    {
                        res.Add(datetime);
                    }
                }

                return res;
            }
        }

        [HttpPost]
        [Authorize(Roles = Roles.AdministratorAndEmployeeAndClient)]
        public List<DateTime> GetAllAvailablesForDay([FromBody] GetHairdressingAppointmentDto getAppointmentDto)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                
                int? userId = GetUserId();

                if(getAppointmentDto.HairdressingId != null)
                    userId = getAppointmentDto.HairdressingId;

                var prof = dbContext.Hairdressing_Professionals.FirstOrDefault(d => d.Id == getAppointmentDto.ProfessionalId);

                if (prof == null)
                {
                    throw new BadRequestException();
                }

                return prof.GetAllAvailablesForDay(getAppointmentDto.Day);
            }
        }

        [HttpPost]
        [Authorize]
        public void RequestAppointmentForNonClient([FromBody] RequestHairdressingAppointmentForNonClientDto requestAppointmentDto)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

                if (requestAppointmentDto.Day.Date < DateTime.Today.Date)
                {
                    throw new BadRequestException(ExceptionMessages.AppointmentCantBeRequested);
                }

                var prof = dbContext.Hairdressing_Professionals.FirstOrDefault(d => d.Id == requestAppointmentDto.ProfessionalId && d.UserId == userId);

                if (prof == null)
                {
                    throw new BadRequestException();
                }

                if (!_roleManager.RoleExistsAsync(Roles.Client).Result)
                {
                    throw new ApplicationException(ExceptionMessages.RolesHaveNotBeenCreated);
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

                var patient = new Hairdressing_Patient
                {
                    
                    UserId = userId,
                    ClientId = client.Id,
                };

                dbContext.Hairdressing_Patients.Add(patient);
                dbContext.SaveChanges();

                var availableAppointments = prof.GetAllAvailablesForDay(requestAppointmentDto.Day.Date);

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
                    State = AppointmentStateEnum.Reserved,
                    PatientId = patient.Id,
                    UserId = userId
                });

                dbContext.SaveChanges();
            }
        }

        [HttpPost]
        [Authorize(Roles = Roles.AdministratorAndEmployee)]
        public void RequestAppointmentForClient([FromBody] RequestHairdressingAppointmentForClientDto requestAppointmentDto)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

                if (requestAppointmentDto.Day.Date < DateTime.Today.Date)
                {
                    throw new BadRequestException(ExceptionMessages.AppointmentCantBeRequested);
                }

                var prof = dbContext.Hairdressing_Professionals.FirstOrDefault(d => d.Id == requestAppointmentDto.ProfessionalId && d.UserId == userId);

                if (prof == null)
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

                var availableAppointments = prof.GetAllAvailablesForDay(requestAppointmentDto.Day.Date);

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
                    State = AppointmentStateEnum.Reserved,
                    PatientId = patient.Id,
                    UserId = userId
                });

                dbContext.SaveChanges();
            }
        }

        [HttpPost]
        [Authorize(Roles = Roles.AdministratorAndEmployee)]
        public void RequestAppointmentForPatient([FromBody] RequestHairdressingAppointmentForPatientDto requestAppointmentDto)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

                if (requestAppointmentDto.Day.Date < DateTime.Today.Date)
                {
                    throw new BadRequestException(ExceptionMessages.AppointmentCantBeRequested);
                }

                var prof = dbContext.Hairdressing_Professionals.FirstOrDefault(d => d.Id == requestAppointmentDto.ProfessionalId && d.UserId == userId);

                if (prof == null)
                {
                    throw new BadRequestException();
                }

                var patient = dbContext.Hairdressing_Patients.FirstOrDefault(p => p.Id == requestAppointmentDto.PatientId && p.UserId == userId);

                if (patient == null)
                {
                    throw new BadRequestException();
                }

                var availableAppointments = prof.GetAllAvailablesForDay(requestAppointmentDto.Day.Date);

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
                    State = AppointmentStateEnum.Reserved,
                    PatientId = patient.Id,
                    UserId = userId
                });

                dbContext.SaveChanges();
            }
        }

        [HttpPost]
        [Authorize(Roles = Roles.Client)]
        public PaymentDto RequestAppointmentByClient([FromBody] RequestHairdressingAppointmentByClientDto requestAppointmentDto)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

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

                var availableAppointments = prof.GetAllAvailablesForDay(requestAppointmentDto.Day.Date);

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
                    PatientId = patient.Id,
                    UserId = hairdressing.UserId
                };

                dbContext.Hairdressing_Appointments.Add(hairdressingAppointment);

                dbContext.SaveChanges();

                var paymentLink = GeneratePaymentLink(hairdressing, hairdressingAppointment, client.User.Email);

                return new PaymentDto
                {
                    PaymentLink = paymentLink
                };
            }
        }

        [HttpPost]
        [Authorize(Roles = Roles.Client)]
        public PaymentDto RequestAppointmentByPatient([FromBody] RequestHairdressingAppointmentByPatientDto requestAppointmentDto)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

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

                var client = dbContext.Clients.FirstOrDefault(c => c.UserId == userId);
                var patient = dbContext.Hairdressing_Patients.FirstOrDefault(p => p.ClientId == client.Id && p.UserId == hairdressing.UserId);

                if (patient == null)
                {
                    throw new BadRequestException();
                }

                var availableAppointments = prof.GetAllAvailablesForDay(requestAppointmentDto.Day.Date);

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
                    PatientId = patient.Id,
                    UserId = hairdressing.UserId
                };

                dbContext.Hairdressing_Appointments.Add(hairdressingAppointment);

                dbContext.SaveChanges();

                var paymentLink = GeneratePaymentLink(hairdressing, hairdressingAppointment, client.User.Email);

                return new PaymentDto
                {
                    PaymentLink = paymentLink
                };
            }
        }

        [HttpPost]
        [Authorize(Roles = Roles.AdministratorAndEmployee)]
        public void CancelAppointmentByHairdressing([FromBody] CancelHairdressingAppointmentDto cancelAppointmentDto)
        {
            var emailMessage = new EmailDto();

            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

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

                emailMessage = new EmailDto
                {
                    From = "no-reply@tuturno.com.ar",
                    Subject = $"{hairdressing.Name} - Cancelacion de turno",
                    To = new List<string> { appointment.Patient.Client.User.Email },
                    Message = cancelAppointmentDto.Comment
                };

                appointment.State = AppointmentStateEnum.Cancelled;
                dbContext.SaveChanges();
            }

            emailService.Send(emailMessage);
        }

        [HttpPost]
        [Authorize]
        public void CompleteAppointmentByHairdressing([FromBody] IdDto completeAppointmentDto)
        {
            var emailMessage = new EmailDto();

            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

                var appointment = dbContext.Hairdressing_Appointments.FirstOrDefault(a => a.Id == completeAppointmentDto.Id && a.UserId == userId);
                var hairdressing = dbContext.Hairdressings.FirstOrDefault(c => c.UserId == appointment.UserId);

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

                appointment.State = AppointmentStateEnum.Completed;

                emailMessage = new EmailDto
                {
                    From = "no-reply@tuturno.com.ar",
                    Subject = "Turno completado",
                    To = new List<string> { appointment.Patient.Client.User.Email, hairdressing.User.Email },
                    Message = $"Se ha completado el turno numero {appointment.Id}."
                };

                dbContext.SaveChanges();
            }

            emailService.Send(emailMessage);
        }

        [HttpPost]
        [Authorize]
        public void CancelAppointment([FromBody] CancelHairdressingAppointmentDto cancelAppointmentDto)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

                var appointment = dbContext.Hairdressing_Appointments.FirstOrDefault(a => a.Id == cancelAppointmentDto.Id && a.UserId == userId);

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

        [HttpPost]
        [Authorize]
        public void CompleteAppointment([FromBody] CompleteHairdressingAppointmentDto completeAppointmentDto)
        {
            var emailMessage = new EmailDto();

            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

                var appointment = dbContext.Hairdressing_Appointments.FirstOrDefault(a => a.Id == completeAppointmentDto.Id && a.UserId == userId);
                var hairdressing = dbContext.Hairdressings.FirstOrDefault(c => c.UserId == appointment.UserId);

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

                appointment.State = AppointmentStateEnum.Completed;

                emailMessage = new EmailDto
                {
                    From = "no-reply@tuturno.com.ar",
                    Subject = "Turno completado",
                    To = new List<string> { appointment.Patient.Client.User.Email, hairdressing.User.Email },
                    Message = $"Se ha completado el turno numero {appointment.Id}."
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

            emailService.Send(emailMessage);
        }

        [HttpPost]
        [Authorize]
        public List<RequestedHairdressingAppointmentsByProfessionalDto> GetRequestedAppointmentsByFilter([FromBody] FilterRequestedHairdressingAppointmentDto filter)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

                var patients = dbContext.Hairdressing_Patients.Where(p => p.UserId == userId).ToList();

                var profs = dbContext.Hairdressing_Professionals
                    .Where(d => d.UserId == userId)
                    .Where(d => !filter.SpecialtyId.HasValue || d.SpecialtyId == filter.SpecialtyId)
                    .Where(d => !filter.SubspecialtyId.HasValue || d.SubspecialtyId == filter.SubspecialtyId)
                    .ToList();

                return profs.Select(d => new RequestedHairdressingAppointmentsByProfessionalDto
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

        [HttpPost]
        [Authorize]
        public List<DayDto> GetWeek([FromBody] FilterWeekHairdressingAppointmentDto filter)
        {
            // Tengo que devolver una lista con todos los dias entre la fecha desde y la fecha hasta
            // Para cada dia, tengo que partirlo en 24 horas
            // Para cada hora tengo que tener una lista con todas las especialidades del usuario
            // Para cada especialidad tengo que decir cuantos turnos reservados tiene en esa fecha y en ese rango de horas
            var res = new List<DayDto>();

            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

                var specialties = dbContext.Hairdressing_Specialties.Where(s => s.UserId == userId).ToList();

                var appointments = dbContext.Hairdressing_Appointments
                    .Where(a => a.Professional.UserId == userId)
                    .Where(a => !filter.ProfessionalId.HasValue || a.ProfessionalId == filter.ProfessionalId)
                    .Where(a => !filter.SubSpecialtyId.HasValue || a.Professional.SubspecialtyId == filter.SubSpecialtyId)
                    .Where(a => !filter.SpecialtyId.HasValue || a.Professional.SpecialtyId == filter.SpecialtyId)
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
                            var count = appointments.Count(a => datetime <= a.DateTime && a.DateTime < nextHour && a.Professional.SpecialtyId == specialty.Id);

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

            return res;
        }

        [HttpPost]
        [Authorize(Roles = Roles.Client)]
        public List<HairdressingClientDayDto> GetWeekForClient([FromBody] FilterClientWeekHairdressingAppointmentDto filter)
        {
            var service = new AppointmentService();
            var week = service.Hairdressing_GetWeekForClient(filter, HttpContext);

            return week;
        }

        [HttpPost]
        public List<HairdressingAppointmentsPerDayDto> GetAvailableAppointmentsPerDay([FromBody] FilterAvailableHairdressingAppointmentDto filter)
        {
            var res = new List<HairdressingAppointmentsPerDayDto>();

            using (var dbContext = new ApplicationDbContext())
            {
                var userId = filter.HairdressingId.HasValue ? dbContext.Hairdressings.Where(c => c.Id == filter.HairdressingId).Select(c => c.UserId).First() : -1;

                var profs = dbContext.Hairdressing_Professionals
                    .Where(d => !filter.ProfessionalId.HasValue || d.UserId == userId)
                    .Where(d => !filter.ProfessionalId.HasValue || d.Id == filter.ProfessionalId)
                    .Where(d => !filter.SubSpecialtyId.HasValue || d.SubspecialtyId == filter.SubSpecialtyId)
                    .Where(d => !filter.SpecialtyId.HasValue || d.SpecialtyId == filter.SpecialtyId)
                    .ToList();

                for (var date = filter.StartDate.Date; date <= filter.EndDate.Date; date = date.AddDays(1))
                {
                    var day = new HairdressingAppointmentsPerDayDto { Day = date, AvailableAppointments = 0 };

                    foreach (var prof in profs)
                    {
                        var availableAppointments = prof.GetAllAvailablesForDay(date);
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

        private string GeneratePaymentLink(Hairdressing hairdressing, Hairdressing_Appointment hairdressingAppointment, string userEmail)
        {
            if (!hairdressing.RequiresPayment)
            {
                return string.Empty;
            }

            return mercadoPagoService.GeneratePaymentLink(new MpRequestDto
            {
                ClientId = hairdressing.ClientId,
                ClientSecret = hairdressing.ClientSecret,
                Title = $"Turno en peluqueria '{hairdressing.Name}' el dia {hairdressingAppointment.DateTime.ToShortDateString()} en el horario {hairdressingAppointment.DateTime.ToShortTimeString()}",
                Price = hairdressingAppointment?.Professional?.Subspecialty?.Price ?? 1,
                BuyerEmail = userEmail,
            });
        }
    }
}
