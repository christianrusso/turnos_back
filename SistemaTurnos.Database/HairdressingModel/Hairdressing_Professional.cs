using SistemaTurnos.Database.Enums;
using SistemaTurnos.Database.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace SistemaTurnos.Database.HairdressingModel
{
    public class Hairdressing_Professional
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
        public HairdressingProfessionalStateEnum State { get; set; }

        public virtual List<Hairdressing_ProfessionalSubspecialty> Subspecialties { get; set; }

        public virtual List<Hairdressing_WorkingHours> WorkingHours { get; set; }

        public virtual List<Hairdressing_Appointment> Appointments { get; set; }

        public virtual List<Hairdressing_BlockedDay> BlockedDays { get; set; }

        [Required]
        public int UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        public List<DateTime> GetAllAvailableAppointmentsForDay(DateTime day, int subspecialtyId)
        {
            var professionalSubspecialty = Subspecialties.First(ssp => ssp.SubspecialtyId == subspecialtyId);
            var consultationTime = TimeSpan.FromMinutes(professionalSubspecialty.ConsultationLength);

            var availableAppointments = new List<DateTime>();

            if (!BlockedDays.Any(bd => bd.SubspecialtyId == subspecialtyId && bd.SameDay(day)))
            {
                availableAppointments = GetAllAppointmentsForDay(day, subspecialtyId);
                var removedAppointments = new List<DateTime>();

                foreach (var availableAppointment in availableAppointments)
                {
                    var availableAppointmentStart = availableAppointment.AddSeconds(1);
                    var availableAppointmentEnd = availableAppointment.Add(consultationTime).AddSeconds(-1);
                    
                    foreach (var appointment in Appointments.Where(a => a.DateTime.Date == day.Date).ToList())
                    {
                        var appointmentTime = TimeSpan.FromMinutes(Subspecialties.First(s => s.SubspecialtyId == appointment.SubspecialtyId).ConsultationLength);
                        var appointmentStart = appointment.DateTime;
                        var appointmentEnd = appointment.DateTime.Add(appointmentTime);

                        if (appointment.State != AppointmentStateEnum.Cancelled && Overlap(availableAppointmentStart, availableAppointmentEnd, appointment.DateTime, appointmentEnd))
                        {
                            removedAppointments.Add(availableAppointment);
                        }
                    }
                }

                foreach (var a in removedAppointments)
                {
                    availableAppointments.Remove(a);
                }
            }

            return availableAppointments;
        }

        private List<DateTime> GetAllAppointmentsForDay(DateTime day, int subspecialtyId)
        {
            var allAppointments = new List<DateTime>();

            if (!BlockedDays.Any(bd => bd.SubspecialtyId == subspecialtyId && bd.SameDay(day)))
            {
                var professionalSubspecialty = Subspecialties.First(ssp => ssp.SubspecialtyId == subspecialtyId);
                day = day.Date;
                var dayNumber = day.DayOfWeek;
                //var consultationTime = TimeSpan.FromMinutes(professionalSubspecialty.ConsultationLength);
                var consultationTime = TimeSpan.FromMinutes(15);
                var dayWorkingHours = WorkingHours.Where(wh => wh.DayNumber == dayNumber).OrderBy(wh => wh.Start).ToList();

                foreach (var wh in dayWorkingHours)
                {
                    var appointmentTime = wh.Start;

                    while (appointmentTime <= wh.End)
                    {
                        allAppointments.Add(new DateTime(day.Year, day.Month, day.Day, appointmentTime.Hours, appointmentTime.Minutes, appointmentTime.Seconds));
                        appointmentTime = appointmentTime.Add(consultationTime);
                    }
                }
            }

            return allAppointments;
        }

        private bool Overlap(DateTime startA, DateTime endA, DateTime startB, DateTime endB)
        {
             return startA.TimeOfDay < endB.TimeOfDay && startB.TimeOfDay < endA.TimeOfDay;
        }
    }
}
