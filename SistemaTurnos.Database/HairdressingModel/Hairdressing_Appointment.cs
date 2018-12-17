using SistemaTurnos.Database.Enums;
using SistemaTurnos.Database.Model;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaTurnos.Database.HairdressingModel
{
    public class Hairdressing_Appointment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int ProfessionalId { get; set; }

        public virtual Hairdressing_Professional Professional { get; set; }

        [Required]
        public DateTime DateTime { get; set; }

        [Required]
        public AppointmentStateEnum State { get; set; }

        public int RatingId { get; set; }

        public virtual Hairdressing_Rating Rating { get; set; }

        public int PatientId { get; set; }

        public virtual Hairdressing_Patient Patient { get; set; }

        public string PreferenceId { get; set; }

        [Required]
        public int UserId { get; set; }

        public virtual ApplicationUser User { get; set; }
    }
}
