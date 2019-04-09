using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SistemaTurnos.Database.HairdressingModel;

namespace SistemaTurnos.Database.Model
{
    public class SystemClient
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        public string Logo { get; set; }

        public virtual ApplicationUser User { get; set; }

        public virtual List<Hairdressing_Patient> HairdressingPatients { get; set; }

        public string FacebookUserId { get; set; }

        public virtual List<Hairdressing_ClientFavorite> FavoriteHairdressing { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string LastName { get; set; }

        [StringLength(500, MinimumLength = 4)]
        public string Address { get; set; }

        [StringLength(50, MinimumLength = 4)]
        public string PhoneNumber { get; set; }

        public string FullName => $"{LastName} {FirstName}";
    }
}
