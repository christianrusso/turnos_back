using System.ComponentModel;
using System.Linq;

namespace SistemaTurnos.WebApplication.WebApi.Extension
{
    public static class ObjectExtensions
    {
        public static string GetEnumDescription(this object value)
        {
            DescriptionAttribute attibute = value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), true).Cast<DescriptionAttribute>().FirstOrDefault();

            if (attibute == null)
            {
                return value.ToString();
            }

            return attibute.Description;
        }
    }
}
