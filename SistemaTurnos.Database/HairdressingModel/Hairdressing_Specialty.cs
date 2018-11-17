using SistemaTurnos.Database.Model;
using SistemaTurnos.Database.ModelData;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaTurnos.Database.HairdressingModel
{
    public class Hairdressing_Specialty
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int DataId { get; set; }

        public virtual SpecialtyData Data { get; set; }

        public virtual List<Hairdressing_Professional> Professionals { get; set; }

        public virtual List<Hairdressing_Subspecialty> Subspecialties { get; set; }

        [Required]
        public int UserId { get; set; }

        public virtual ApplicationUser User { get; set; }
    }
}
