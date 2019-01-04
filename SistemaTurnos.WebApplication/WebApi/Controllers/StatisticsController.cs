using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using SistemaTurnos.Commons.Exceptions;
using SistemaTurnos.Database;
using SistemaTurnos.Database.ClinicModel;
using SistemaTurnos.Database.Enums;
using SistemaTurnos.Database.HairdressingModel;
using SistemaTurnos.WebApplication.WebApi.Dto.Statistics;
using SistemaTurnos.WebApplication.WebApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SistemaTurnos.WebApplication.WebApi.Controllers
{
    [Route("Api/[controller]/[action]")]
    [Produces("application/json")]
    [EnableCors("AnyOrigin")]
    public class StatisticsController : Controller
    {
        [HttpGet]
        [Authorize]
        public StatisticsFullDto GetForClinic()
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = new BusinessPlaceService().GetUserId(HttpContext);

                if (!dbContext.Clinics.Any(c => c.UserId == userId))
                {
                    throw new BadRequestException();
                }

                var now = DateTime.Now;
                var todayStart = DateTime.Today;
                var todayEnd = DateTime.Today.AddHours(23).AddMinutes(59).AddSeconds(59);
                var appointments = dbContext.Clinic_Appointments.Where(a => a.UserId == userId).ToList();

                return new StatisticsFullDto
                {
                    Professionals = dbContext.Clinic_Doctors.Count(d => d.UserId == userId),
                    ActiveProfessionals = dbContext.Clinic_Doctors.Count(d => d.UserId == userId && IsActive(d.WorkingHours, now)),
                    Patients = dbContext.Clinic_Patients.Count(p => p.UserId == userId),
                    Specialties = dbContext.Clinic_Specialties.Count(s => s.UserId == userId),
                    MedicalInsurances = dbContext.Clinic_MedicalInsurances.Count(mi => mi.UserId == userId),
                    Appointments = appointments.Count(),
                    CompletedAppointments = appointments.Count(a => a.State == AppointmentStateEnum.Completed),
                    CanceledAppointments = appointments.Count(a => a.State == AppointmentStateEnum.Cancelled),
                    TodayAppointments = appointments.Count(a => todayStart <= a.DateTime && a.DateTime <= todayEnd),
                    PanelAppointments = appointments.Count(a => a.Source == AppointmentSourceEnum.Panel),
                    WebAppointments = appointments.Count(a => a.Source == AppointmentSourceEnum.Web),
                    MobileAppointments = appointments.Count(a => a.Source == AppointmentSourceEnum.Mobile),
                };
            }
        }

        [HttpGet]
        [Authorize]
        public StatisticsFullDto GetForHairdressing()
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var userId = new BusinessPlaceService().GetUserId(HttpContext);

                if (!dbContext.Hairdressings.Any(c => c.UserId == userId))
                {
                    throw new BadRequestException();
                }

                var now = DateTime.Now;
                var todayStart = DateTime.Today;
                var todayEnd = DateTime.Today.AddHours(23).AddMinutes(59).AddSeconds(59);
                var appointments = dbContext.Hairdressing_Appointments.Where(a => a.UserId == userId).ToList();

                return new StatisticsFullDto
                {
                    Professionals = dbContext.Hairdressing_Professionals.Count(d => d.UserId == userId),
                    ActiveProfessionals = dbContext.Hairdressing_Professionals.Count(d => d.UserId == userId && IsActive(d.WorkingHours, now)),
                    Patients = dbContext.Hairdressing_Patients.Count(p => p.UserId == userId),
                    Specialties = dbContext.Hairdressing_Specialties.Count(s => s.UserId == userId),
                    MedicalInsurances = 0,
                    Appointments = appointments.Count(),
                    CompletedAppointments = appointments.Count(a => a.State == AppointmentStateEnum.Completed),
                    CanceledAppointments = appointments.Count(a => a.State == AppointmentStateEnum.Cancelled),
                    TodayAppointments = appointments.Count(a => todayStart <= a.DateTime && a.DateTime <= todayEnd),
                    PanelAppointments = appointments.Count(a => a.Source == AppointmentSourceEnum.Panel),
                    WebAppointments = appointments.Count(a => a.Source == AppointmentSourceEnum.Web),
                    MobileAppointments = appointments.Count(a => a.Source == AppointmentSourceEnum.Mobile),
                };
            }
        }

        private bool IsActive(List<Clinic_WorkingHours> workingHours, DateTime dateTime)
        {
            return workingHours.Any(wh => wh.DayNumber == dateTime.DayOfWeek && wh.Start <= dateTime.TimeOfDay && dateTime.TimeOfDay <= wh.End);
        }

        private bool IsActive(List<Hairdressing_WorkingHours> workingHours, DateTime dateTime)
        {
            return workingHours.Any(wh => wh.DayNumber == dateTime.DayOfWeek && wh.Start <= dateTime.TimeOfDay && dateTime.TimeOfDay <= wh.End);
        }
    }
}
