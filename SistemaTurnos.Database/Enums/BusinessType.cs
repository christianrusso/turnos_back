using System.ComponentModel;

namespace SistemaTurnos.Database.Enums
{
    public enum BusinessType : int
    {
        [Description("Clinic")]
        Clinic = 1,

        [Description("Hairdressing")]
        Hairdressing = 2,

        [Description("Babershop")]
        Babershop = 3,

        [Description("Esthetic")]
        Esthetic = 4
    }

    public static class BusinessTypeMethods
    {
        public static bool IsClinic(this BusinessType businessType)
        {
            return businessType == BusinessType.Clinic;
        }

        public static bool IsHBE(this BusinessType businessType)
        {
            return businessType == BusinessType.Hairdressing || businessType == BusinessType.Babershop || businessType == BusinessType.Esthetic;
        }
    }
}
