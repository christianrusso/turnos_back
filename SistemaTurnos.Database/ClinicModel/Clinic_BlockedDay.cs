using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaTurnos.Database.ClinicModel
{
    public class Clinic_BlockedDay
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public DateTime DateTime { get; set; }

        [Required]
        public int DoctorId { get; set; }

        public virtual Clinic_Doctor Doctor { get; set; }

        [Required]
        public int SubspecialtyId { get; set; }

        public virtual Clinic_Subspecialty Subspecialty { get; set; }

        public bool SameDay(DateTime day)
        {
            var start = new DateTime(DateTime.Year, DateTime.Month, DateTime.Day, 0, 0, 0);
            var end = new DateTime(DateTime.Year, DateTime.Month, DateTime.Day, 23, 59, 59);
            return start <= day && day <= end;
        }
    }
}
