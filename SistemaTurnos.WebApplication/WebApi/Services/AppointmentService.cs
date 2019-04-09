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
                            Specialty = dayAppointment.Subspecialty.Specialty.Data.Description,
                            Subspecialty = dayAppointment.Subspecialty?.Data.Description ?? string.Empty,
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
            var weekHairdressing = this.Hairdressing_GetWeekForClient(new FilterClientWeekHairdressingAppointmentDto
            {
                EndDate = filter.EndDate,
                StartDate = filter.StartDate
            }, httpContext);

            var week = new WeekForClientDto();

            week.Hairdressing_GetWeekForClient = weekHairdressing;

            return week;
        }
    }
}
