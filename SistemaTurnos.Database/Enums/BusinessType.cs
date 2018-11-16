using System.ComponentModel;

namespace SistemaTurnos.Database.Enums
{
    public enum BusinessType : int
    {
        [Description("Clinic")]
        Clinic = 1,
        [Description("Hairdressing")]
        Hairdressing = 2
    }
}
