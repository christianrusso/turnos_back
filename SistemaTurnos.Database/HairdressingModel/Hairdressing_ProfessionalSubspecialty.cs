using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaTurnos.Database.HairdressingModel
{
    public class Hairdressing_ProfessionalSubspecialty
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int ProfessionalId { get; set; }

        public virtual Hairdressing_Professional Professional { get; set; }

        [Required]
        public int SubspecialtyId { get; set; }

        public virtual Hairdressing_Subspecialty Subspecialty { get; set; }

        [Required]
        public uint ConsultationLength { get; set; }
    }
}
