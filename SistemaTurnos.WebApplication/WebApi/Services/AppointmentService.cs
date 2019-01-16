using Microsoft.AspNetCore.Http;
using SistemaTurnos.Database;
using SistemaTurnos.WebApplication.WebApi.Dto.Appointment;
using SistemaTurnos.WebApplication.WebApi.Dto.Common;
using SistemaTurnos.WebApplication.WebApi.Dto.HairdressingAppointment;
using System.Collections.Generic;
using System.Linq;

namespace SistemaTurnos.WebApplication.WebApi.Services
{
    public class AppointmentService:ServiceBase
    {
        public List<ClientDayDto> Clinic_GetWeekForClient(FilterClientWeekAppointmentDto filter, HttpContext httpContext)
        {
            var res = new List<ClientDayDto>();

            using (var dbContext = new ApplicationDbContext())
            {
                var userId = GetUserId(httpContext);

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
                            Logo = clinic.Logo,
                            Doctor = $"{dayAppointment.Doctor.FirstName} {dayAppointment.Doctor.LastName}",
                            Specialty = dayAppointment.Doctor.Specialty.Data.Description,
                            Subspecialty = dayAppointment.Doctor.Subspecialty?.Data.Description ?? string.Empty,
                            DateTime = dayAppointment.DateTime,
                            Id = dayAppointment.Id,
                            State = dayAppointment.State,
                        };

                        day.Appointments.Add(appointmentInformation);
                    }
                    res.Add(day);
                }
            }

            return res;
        }

        internal List<HairdressingClientDayDto> Hairdressing_GetWeekForClient(FilterClientWeekHairdressingAppointmentDto filter, HttpContext httpContext)
        {
            var res = new List<HairdressingClientDayDto>();

            using (var dbContext = new ApplicationDbContext())
            {
                var userId = this.GetUserId(httpContext);

                var hairdressings = dbContext.Hairdressings.ToList();

                var appointments = dbContext.Hairdressing_Appointments
                    .Where(a => a.Patient.Client.UserId == userId)
                    .ToList();

                for (var date = filter.StartDate.Date; date <= filter.EndDate.Date; date = date.AddDays(1))
                {
                    var day = new HairdressingClientDayDto { Day = date, Appointments = new List<PatientHairdressingAppointmentInformationDto>() };
                    var nextDate = date.AddDays(1);

                    var dayAppointments = appointments.Where(a => day.Day <= a.DateTime && a.DateTime < nextDate).OrderBy(a => a.DateTime).ToList();

                    foreach (var dayAppointment in dayAppointments)
                    {
                        var hairdressing = hairdressings.First(c => c.UserId == dayAppointment.UserId);

                        var appointmentInformation = new PatientHairdressingAppointmentInformationDto
                        {
                            HairdressingId = hairdressing.Id,
                            Hairdressing = hairdressing.Name,
                            Logo = hairdressing.Logo,
                            Professional = $"{dayAppointment.Professional.FirstName} {dayAppointment.Professional.LastName}",
                            Specialty = dayAppointment.Professional.Specialty.Data.Description,
                            Subspecialty = dayAppointment.Professional.Subspecialty?.Data.Description ?? string.Empty,
                            DateTime = dayAppointment.DateTime,
                            Id = dayAppointment.Id,
                            State = dayAppointment.State
                        };

                        day.Appointments.Add(appointmentInformation);
                    }
                    res.Add(day);
                }
            }

            return res;

        }

        internal WeekForClientDto GetWeekForClient(FilterClientWeekDto filter, HttpContext httpContext)
        {
            var weekClinic = this.Clinic_GetWeekForClient(new FilterClientWeekAppointmentDto()
            {
                EndDate = filter.EndDate,
                StartDate = filter.StartDate
            }, httpContext);

            var weekHairdressing = this.Hairdressing_GetWeekForClient(new FilterClientWeekHairdressingAppointmentDto
            {
                EndDate = filter.EndDate,
                StartDate = filter.StartDate
            }, httpContext);

            var week = new WeekForClientDto();

            week.Clinic_GetWeekForClient = weekClinic;
            week.Hairdressing_GetWeekForClient = weekHairdressing;

            return week;
        }
    }
}
