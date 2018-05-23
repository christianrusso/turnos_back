using System.ComponentModel;

namespace SistemaTurnos.WebApplication.Database.Enums
{
    public enum AppointmentStateEnum : int
    {
        [Description("Reservado")]
        Reserved = 1,

        [Description("Cancelado")]
        Cancelled = 2,

        [Description("Completado")]
        Completed = 3
    }
}
