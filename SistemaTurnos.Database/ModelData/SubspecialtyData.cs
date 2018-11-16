using SistemaTurnos.Database.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaTurnos.Database.ModelData
{
    public class SubspecialtyData
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Description { get; set; }

        [Required]
        public int SpecialtyDataId { get; set; }

        [Required]
        public virtual SpecialtyData SpecialtyData { get; set; }

        [Required]
        public RubroEnum Rubro { get; set; }
    }
}
