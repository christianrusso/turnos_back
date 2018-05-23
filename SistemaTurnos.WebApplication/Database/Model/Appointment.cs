using SistemaTurnos.WebApplication.Database.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SistemaTurnos.WebApplication.Database.Model
{
    public class Appointment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int DoctorId { get; set; }

        public Doctor Doctor { get; set; }

        [Required]
        public DateTime DateTime { get; set; }

        [Required]
        public AppointmentStateEnum State { get; set; }

        public int RatingId { get; set; }

        public Rating Rating { get; set; }

        public int PatientId { get; set; }

        public Patient Patient { get; set; }

        [Required]
        public int UserId { get; set; }

        public ApplicationUser User { get; set; }
    }
}
