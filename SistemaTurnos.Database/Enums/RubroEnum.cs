using System.ComponentModel;

namespace SistemaTurnos.Database.Enums
{
    public enum RubroEnum : int
    {
        [Description("Clinic")]
        Clinic = 1,

        [Description("Hairdressing")]
        Hairdressing = 2,

        [Description("Unknown")]
        Unknown = 999,
    }
}
