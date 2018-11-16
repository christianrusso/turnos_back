using System.ComponentModel;

namespace SistemaTurnos.Database.Enums
{
    public enum DoctorStateEnum : int
    {
        [Description("Activo")]
        Active = 1,

        [Description("Inactivo")]
        Inactive = 2,

        [Description("Vacaciones")]
        Vacation = 3
    }
}
