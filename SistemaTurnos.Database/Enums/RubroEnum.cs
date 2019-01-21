using System.ComponentModel;

namespace SistemaTurnos.Database.Enums
{
    public enum RubroEnum : int
    {
        [Description("Clinic")]
        Clinic = 1,

        [Description("Hairdressing")]
        Hairdressing = 2,
        
        [Description("Babershop")]
        Babershop = 3,

        [Description("Esthetic")]
        Esthetic = 4,

        [Description("Unknown")]
        Unknown = 999,
    }
}
