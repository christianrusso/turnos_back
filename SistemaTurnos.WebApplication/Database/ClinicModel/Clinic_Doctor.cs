using SistemaTurnos.WebApplication.Database.Enums;
using SistemaTurnos.WebApplication.Database.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace SistemaTurnos.WebApplication.Database.ClinicModel
{
    public class Clinic_Doctor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        [MaxLength(50)]
        public string Email { get; set; }

        [MaxLength(50)]
        public string PhoneNumber { get; set; }

        [Required]
        public uint ConsultationLength { get; set; }

        [Required]
        public int SpecialtyId { get; set; }

        [Required]
        public virtual Clinic_Specialty Specialty { get; set; }

        [Required]
        public DoctorStateEnum State { get; set; }

        public int? SubspecialtyId { get; set; }

        public virtual Clinic_Subspecialty Subspecialty { get; set; }

        public virtual List<Clinic_WorkingHours> WorkingHours { get; set; }

        public virtual List<Clinic_Appointment> Appointments { get; set; }

        [Required]
        public int UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        public List<DateTime> GetAvailableAppointmentsForDay(DateTime day)
        {
            day = day.Date;
            var dayNumber = day.DayOfWeek;
            var consultationMinutes = TimeSpan.FromMinutes(ConsultationLength);
            var dayWorkingHours = WorkingHours.Where(wh => wh.DayNumber == dayNumber).OrderBy(wh => wh.Start).ToList();
            var allAppointments = new List<DateTime>();

            foreach (var wh in dayWorkingHours)
            {
                var appointmentTime = wh.Start;

                while (appointmentTime <= wh.End)
                {
                    allAppointments.Add(new DateTime(day.Year, day.Month, day.Day, appointmentTime.Hours, appointmentTime.Minutes, appointmentTime.Seconds));
                    appointmentTime = appointmentTime.Add(consultationMinutes);
                }
            }

            return allAppointments;
        }

        public List<DateTime> GetAllAvailablesForDay(DateTime day)
        {
            var availableAppointments = GetAvailableAppointmentsForDay(day);

            foreach (var appointment in Appointments)
            {
                availableAppointments.Remove(appointment.DateTime);
            }

            return availableAppointments;
        }
    }
}
