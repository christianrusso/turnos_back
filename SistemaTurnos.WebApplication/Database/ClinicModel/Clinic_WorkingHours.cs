using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaTurnos.WebApplication.Database.ClinicModel
{
    public class Clinic_WorkingHours
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Range(0, 6)]
        public DayOfWeek DayNumber { get; set; }

        [Required]
        public TimeSpan Start { get; set; }

        [Required]
        public TimeSpan End { get; set; }

        [Required]
        public int DoctorId { get; set; }
        
        public virtual Clinic_Doctor Doctor { get; set; }
    }
}
