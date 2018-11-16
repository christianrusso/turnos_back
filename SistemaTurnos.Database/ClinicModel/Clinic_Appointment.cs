using SistemaTurnos.Database.Enums;
using SistemaTurnos.Database.Model;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaTurnos.Database.ClinicModel
{
    public class Clinic_Appointment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int DoctorId { get; set; }

        public virtual Clinic_Doctor Doctor { get; set; }

        [Required]
        public DateTime DateTime { get; set; }

        [Required]
        public AppointmentStateEnum State { get; set; }

        public int RatingId { get; set; }

        public virtual Clinic_Rating Rating { get; set; }

        public int PatientId { get; set; }

        public virtual Clinic_Patient Patient { get; set; }

        [Required]
        public int UserId { get; set; }

        public virtual ApplicationUser User { get; set; }
    }
}
