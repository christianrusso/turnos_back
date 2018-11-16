using SistemaTurnos.Database.ClinicModel;
using System.ComponentModel.DataAnnotations;

namespace SistemaTurnos.Database.HairdressingModel
{
    public class Hairdressing_ClientFavorite
    {
        [Required]
        public int ClientId { get; set; }

        public virtual SystemClient Client { get; set; }

        [Required]
        public int HairdressingId { get; set; }

        public virtual Hairdressing Hairdressing { get; set; }
    }
}
