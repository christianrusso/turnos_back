using SistemaTurnos.WebApplication.Database.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaTurnos.WebApplication.Database.ModelData
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
        public RubroEnum Rubro { get; set; }
        
        public virtual List<SubspecialtyData> Subspecialties { get; set; }
    }
}
