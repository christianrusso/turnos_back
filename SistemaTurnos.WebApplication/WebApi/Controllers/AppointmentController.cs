using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaTurnos.WebApplication.Database;
using SistemaTurnos.WebApplication.Database.ClinicModel;
using SistemaTurnos.WebApplication.Database.Enums;
using SistemaTurnos.WebApplication.Database.Model;
using SistemaTurnos.WebApplication.Email;
using SistemaTurnos.WebApplication.WebApi.Authorization;
using SistemaTurnos.WebApplication.WebApi.Dto;
using SistemaTurnos.WebApplication.WebApi.Dto.Appointment;
using SistemaTurnos.WebApplication.WebApi.Exceptions;
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

        public AppointmentController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

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
                    throw new BadRequestException(ExceptionMessages.BadRequest);
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

        [HttpPost]
        [Authorize(Roles = Roles.AdministratorAndEmployeeAndClient)]
        public List<DateTime> GetAllAvailablesForDay([FromBody] GetAppointmentDto getAppointmentDto)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                
                int? userId = GetUserId();

                if(getAppointmentDto.ClinicId != null)
                    userId = getAppointmentDto.ClinicId;

                var doctor = dbContext.Clinic_Doctors.FirstOrDefault(d => d.Id == getAppointmentDto.DoctorId && d.UserId == userId);

                if (doctor == null)
                {
                    throw new BadRequestException(ExceptionMessages.BadRequest);
                }

                return doctor.GetAllAvailablesForDay(getAppointmentDto.Day);
            }
        }

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
                    throw new BadRequestException(ExceptionMessages.BadRequest);
                }

                var medicalPlan = dbContext.Clinic_MedicalPlans.FirstOrDefault(mp => mp.Id == requestAppointmentDto.MedicalPlanId);

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
                    UserId = appUser.Id
                };

                dbContext.Clients.Add(client);
                dbContext.SaveChanges();

                var patient = new Clinic_Patient
                {
                    FirstName = requestAppointmentDto.FirstName,
                    LastName = requestAppointmentDto.LastName,
                    Address = requestAppointmentDto.Address,
                    PhoneNumber = requestAppointmentDto.PhoneNumber,
                    Dni = requestAppointmentDto.Dni,
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
                    PatientId = patient.Id,
                    UserId = userId
                });

                dbContext.SaveChanges();
            }
        }

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
                    throw new BadRequestException(ExceptionMessages.BadRequest);
                }

                var client = dbContext.Clients.FirstOrDefault(c => c.Id == requestAppointmentDto.ClientId);

                if (client == null)
                {
                    throw new BadRequestException(ExceptionMessages.BadRequest);
                }

                var medicalPlan = dbContext.Clinic_MedicalPlans.FirstOrDefault(mp => mp.Id == requestAppointmentDto.MedicalPlanId);

                if (medicalPlan == null)
                {
                    throw new ApplicationException(ExceptionMessages.BadRequest);
                }

                var patient = dbContext.Clinic_Patients.FirstOrDefault(p => p.ClientId == client.Id && p.UserId == userId);

                if (patient != null)
                {
                    throw new ApplicationException(ExceptionMessages.BadRequest);
                }

                patient = new Clinic_Patient
                {
                    FirstName = requestAppointmentDto.FirstName,
                    LastName = requestAppointmentDto.LastName,
                    Address = requestAppointmentDto.Address,
                    PhoneNumber = requestAppointmentDto.PhoneNumber,
                    Dni = requestAppointmentDto.Dni,
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
                    throw new BadRequestException(ExceptionMessages.AppointmentAlreadyTaken);
                }

                dbContext.Clinic_Appointments.Add(new Clinic_Appointment
                {
                    DoctorId = requestAppointmentDto.DoctorId,
                    Doctor = doctor,
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
                    throw new BadRequestException(ExceptionMessages.BadRequest);
                }

                var patient = dbContext.Clinic_Patients.FirstOrDefault(p => p.Id == requestAppointmentDto.PatientId && p.UserId == userId);

                if (patient == null)
                {
                    throw new BadRequestException(ExceptionMessages.BadRequest);
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
                    PatientId = patient.Id,
                    UserId = userId
                });

                dbContext.SaveChanges();
            }
        }

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
                    throw new BadRequestException(ExceptionMessages.BadRequest);
                }

                var doctor = dbContext.Clinic_Doctors.FirstOrDefault(d => d.Id == requestAppointmentDto.DoctorId && d.UserId == clinic.UserId);

                if (doctor == null)
                {
                    throw new BadRequestException(ExceptionMessages.BadRequest);
                }

                var client = dbContext.Clients.FirstOrDefault(c => c.UserId == userId);

                if (client == null)
                {
                    throw new BadRequestException(ExceptionMessages.BadRequest);
                }

                var medicalPlan = dbContext.Clinic_MedicalPlans.FirstOrDefault(mp => mp.Id == requestAppointmentDto.MedicalPlanId && mp.UserId == clinic.UserId);

                if (medicalPlan == null)
                {
                    throw new ApplicationException(ExceptionMessages.BadRequest);
                }

                var patient = dbContext.Clinic_Patients.FirstOrDefault(p => p.ClientId == client.Id && p.UserId == clinic.UserId);

                if (patient != null)
                {
                    throw new ApplicationException(ExceptionMessages.BadRequest);
                }

                patient = new Clinic_Patient
                {
                    FirstName = requestAppointmentDto.FirstName,
                    LastName = requestAppointmentDto.LastName,
                    Address = requestAppointmentDto.Address,
                    PhoneNumber = requestAppointmentDto.PhoneNumber,
                    Dni = requestAppointmentDto.Dni,
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
                    PatientId = patient.Id,
                    UserId = clinic.UserId
                });

                dbContext.SaveChanges();
            }
        }

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
                    throw new BadRequestException(ExceptionMessages.BadRequest);
                }

                var doctor = dbContext.Clinic_Doctors.FirstOrDefault(d => d.Id == requestAppointmentDto.DoctorId && d.UserId == clinic.UserId);

                if (doctor == null)
                {
                    throw new BadRequestException(ExceptionMessages.BadRequest);
                }

                var client = dbContext.Clients.FirstOrDefault(c => c.UserId == userId);
                var patient = dbContext.Clinic_Patients.FirstOrDefault(p => p.ClientId == client.Id && p.UserId == clinic.UserId);

                if (patient == null)
                {
                    throw new BadRequestException(ExceptionMessages.BadRequest);
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
                    PatientId = patient.Id,
                    UserId = clinic.UserId
                });

                dbContext.SaveChanges();
            }
        }

        [HttpPost]
        [Authorize(Roles = Roles.AdministratorAndEmployee)]
        public void CancelAppointmentByClinic([FromBody] CancelAppointmentDto cancelAppointmentDto)
        {
            var emailMessage = new EmailMessage();

            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

                var appointment = dbContext.Clinic_Appointments.FirstOrDefault(a => a.Id == cancelAppointmentDto.Id && a.UserId == userId);

                if (appointment == null)
                {
                    throw new BadRequestException(ExceptionMessages.BadRequest);
                }

                var clinic = dbContext.Clinics.FirstOrDefault(c => c.UserId == userId);

                if (clinic == null)
                {
                    throw new BadRequestException(ExceptionMessages.BadRequest);
                }
                
                var maxCancelDateTime = appointment.DateTime.AddHours(-24);

                if (DateTime.Now >= maxCancelDateTime)
                {
                    throw new BadRequestException(ExceptionMessages.AppointmentCantBeCanceled);
                }

                emailMessage = new EmailMessage
                {
                    Subject = $"{clinic.Name} - Cancelacion de turno",
                    To = new List<string> { appointment.Patient.Client.User.Email },
                    Message = cancelAppointmentDto.Comment
                };

                appointment.State = AppointmentStateEnum.Cancelled;
                dbContext.SaveChanges();
            }

            EmailSender.Send(emailMessage);
        }

        [HttpPost]
        [Authorize]
        public void CompleteAppointmentByClinic([FromBody] IdDto completeAppointmentDto)
        {
            var emailMessage = new EmailMessage();

            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

                var appointment = dbContext.Clinic_Appointments.FirstOrDefault(a => a.Id == completeAppointmentDto.Id && a.UserId == userId);
                var clinic = dbContext.Clinics.FirstOrDefault(c => c.UserId == appointment.UserId);

                if (appointment == null)
                {
                    throw new BadRequestException(ExceptionMessages.BadRequest);
                }

                if (appointment.DateTime > DateTime.Now)
                {
                    throw new BadRequestException(ExceptionMessages.AppointmentCantBeCompleted);
                }

                if (clinic == null)
                {
                    throw new BadRequestException(ExceptionMessages.BadRequest);
                }

                appointment.State = AppointmentStateEnum.Completed;

                emailMessage = new EmailMessage
                {
                    Subject = "Turno completado",
                    To = new List<string> { appointment.Patient.Client.User.Email, clinic.User.Email },
                    Message = $"Se ha completado el turno numero {appointment.Id}."
                };

                dbContext.SaveChanges();
            }

            EmailSender.Send(emailMessage);
        }

        [HttpPost]
        [Authorize]
        public void CancelAppointment([FromBody] CancelAppointmentDto cancelAppointmentDto)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

                var appointment = dbContext.Clinic_Appointments.FirstOrDefault(a => a.Id == cancelAppointmentDto.Id && a.UserId == userId);

                if (appointment == null)
                {
                    throw new BadRequestException(ExceptionMessages.BadRequest);
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
        public void CompleteAppointment([FromBody] CompleteAppointmentDto completeAppointmentDto)
        {
            var emailMessage = new EmailMessage();

            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

                var appointment = dbContext.Clinic_Appointments.FirstOrDefault(a => a.Id == completeAppointmentDto.Id && a.UserId == userId);
                var clinic = dbContext.Clinics.FirstOrDefault(c => c.UserId == appointment.UserId);

                if (appointment == null)
                {
                    throw new BadRequestException(ExceptionMessages.BadRequest);
                }

                if (appointment.DateTime > DateTime.Now)
                {
                    throw new BadRequestException(ExceptionMessages.AppointmentCantBeCompleted);
                }

                if (clinic == null)
                {
                    throw new BadRequestException(ExceptionMessages.BadRequest);
                }

                appointment.State = AppointmentStateEnum.Completed;

                emailMessage = new EmailMessage
                {
                    Subject = "Turno completado",
                    To = new List<string> { appointment.Patient.Client.User.Email, clinic.User.Email },
                    Message = $"Se ha completado el turno numero {appointment.Id}."
                };

                appointment.Rating = new Clinic_Rating
                {
                    AppointmentId = appointment.Id,
                    Score = completeAppointmentDto.Score,
                    Comment = completeAppointmentDto.Comment,
                    UserId = userId
                };

                dbContext.SaveChanges();
            }

            EmailSender.Send(emailMessage);
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

        [HttpPost]
        [Authorize(Roles = Roles.Client)]
        public List<ClientDayDto> GetWeekForClient([FromBody] FilterClientWeekAppointmentDto filter)
        {
            var res = new List<ClientDayDto>();

            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId();

                var clinics = dbContext.Clinics.ToList();

                var appointments = dbContext.Clinic_Appointments
                    .Where(a => a.Patient.Client.UserId == userId)
                    .ToList();

                for (var date = filter.StartDate.Date; date <= filter.EndDate.Date; date = date.AddDays(1))
                {
                    var day = new ClientDayDto { Day = date, Appointments = new List<PatientAppointmentInformationDto>() };
                    var nextDate = date.AddDays(1);

                    var dayAppointments = appointments.Where(a => day.Day <= a.DateTime && a.DateTime < nextDate).OrderBy(a => a.DateTime).ToList();

                    foreach (var dayAppointment in dayAppointments)
                    {
                        var clinic = clinics.First(c => c.UserId == dayAppointment.UserId);

                        var appointmentInformation = new PatientAppointmentInformationDto
                        {
                            ClinicId = clinic.Id,
                            Clinic = clinic.Name,
                            Doctor = $"{dayAppointment.Doctor.FirstName} {dayAppointment.Doctor.LastName}",
                            Specialty = dayAppointment.Doctor.Specialty.Data.Description,
                            Subspecialty = dayAppointment.Doctor.Subspecialty?.Data.Description ?? string.Empty,
                            DateTime = dayAppointment.DateTime,
                        };

                        day.Appointments.Add(appointmentInformation);
                    }
                    res.Add(day);
                }
            }

            return res;
        }

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
