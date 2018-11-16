using SistemaTurnos.Database.Model;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaTurnos.Database.HairdressingModel
{
    public class Hairdressing_Employee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        [Required]
        public int OwnerUserId { get; set; }

        public virtual ApplicationUser OwnerUser { get; set; }
    }
}
