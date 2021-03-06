﻿using SistemaTurnos.WebApplication.Database.Model;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaTurnos.WebApplication.Database.ClinicModel
{
    public class Clinic_Rating
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Range(0, 10)]
        public uint Score { get; set; }

        [Required]
        public string Comment { get; set; }

        [Required]
        public DateTime DateTime { get; set; }

        [Required]
        public int AppointmentId { get; set; }

        public virtual Clinic_Appointment Appointment { get; set; }

        [Required]
        public int UserId { get; set; }

        public virtual ApplicationUser User { get; set; }
    }
}
