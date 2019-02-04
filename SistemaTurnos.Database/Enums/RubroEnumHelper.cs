namespace SistemaTurnos.Database.Enums
{
    public class RubroEnumHelper
    {
        public static RubroEnum GetRubro(int id)
        {
            if (id == 1) return RubroEnum.Clinic;
            if (id == 2) return RubroEnum.Hairdressing;
            if (id == 3) return RubroEnum.Babershop;
            if (id == 4) return RubroEnum.Esthetic;

            return RubroEnum.Unknown;
        }
    }
}
