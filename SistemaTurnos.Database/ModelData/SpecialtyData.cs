using SistemaTurnos.Database.HairdressingModel;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaTurnos.Database.ModelData
{
    public class SpecialtyData
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Description { get; set; }

        [Required]
        public int BusinessTypeId { get; set; }

        public virtual BusinessType BusinessType { get; set; }
        
        public virtual List<SubspecialtyData> Subspecialties { get; set; }
    }
}
