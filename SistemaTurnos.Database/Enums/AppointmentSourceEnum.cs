using System.ComponentModel;

namespace SistemaTurnos.Database.Enums
{
    public enum AppointmentSourceEnum : int
    {
        [Description("Panel")]
        Panel = 1,

        [Description("Web")]
        Web = 2,

        [Description("Mobile")]
        Mobile = 3,
    }
}
