using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaTurnos.Database.ClinicModel
{
    public class Clinic_DoctorSubspecialty
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int DoctorId { get; set; }

        public virtual Clinic_Doctor Doctor { get; set; }

        [Required]
        public int SubspecialtyId { get; set; }

        public virtual Clinic_Subspecialty Subspecialty { get; set; }

        [Required]
        public uint ConsultationLength { get; set; }
    }
}
