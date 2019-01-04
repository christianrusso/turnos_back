using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SistemaTurnos.Database.HairdressingModel;
using SistemaTurnos.Database.Model;

namespace SistemaTurnos.Database.ClinicModel
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

        public virtual List<Clinic_Patient> Patients { get; set; }

        public virtual List<Hairdressing_Patient> HairdressingPatients { get; set; }

        public string FacebookUserId { get; set; }

        public virtual List<Clinic_ClientFavorite> FavoriteClinics { get; set; }

        public virtual List<Hairdressing_ClientFavorite> FavoriteHairdressing { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 4)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 4)]
        public string LastName { get; set; }

        [Required]
        [StringLength(500, MinimumLength = 4)]
        public string Address { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 4)]
        public string PhoneNumber { get; set; }

        [StringLength(50, MinimumLength = 4)]
        public string Dni { get; set; }

        public string FullName => $"{LastName} {FirstName}";
    }
}
