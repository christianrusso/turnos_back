﻿using SistemaTurnos.WebApplication.Database.Model;
using SistemaTurnos.WebApplication.Database.ModelData;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaTurnos.WebApplication.Database.ClinicModel
{
    public class Clinic_Subspecialty
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int DataId { get; set; }

        public virtual SubspecialtyData Data { get; set; }

        [Required]
        public uint ConsultationLength { get; set; }

        [Required]
        public int SpecialtyId { get; set; }

        [Required]
        public virtual Clinic_Specialty Specialty { get; set; }

        [Required]
        public int UserId { get; set; }

        public virtual ApplicationUser User { get; set; }
    }
}
