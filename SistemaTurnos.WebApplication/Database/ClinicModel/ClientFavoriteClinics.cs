using System.ComponentModel.DataAnnotations;

namespace SistemaTurnos.WebApplication.Database.ClinicModel
{
    public class Clinic_ClientFavorite
    {
        [Required]
        public int ClientId { get; set; }

        public virtual SystemClient Client { get; set; }

        [Required]
        public int ClinicId { get; set; }

        public virtual Clinic Clinic { get; set; }
    }
}
