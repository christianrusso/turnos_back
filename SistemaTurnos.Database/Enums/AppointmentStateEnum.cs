using System.ComponentModel;

namespace SistemaTurnos.Database.Enums
{
    public enum AppointmentStateEnum : int
    {
        [Description("Reservado")]
        Reserved = 1,

        [Description("Cancelado")]
        Cancelled = 2,

        [Description("Completado")]
        Completed = 3,

        [Description("Pagado")]
        Paid = 4
    }
}
