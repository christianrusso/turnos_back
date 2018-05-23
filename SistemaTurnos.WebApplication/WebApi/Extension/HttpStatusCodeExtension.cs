using System.Net;

namespace SistemaTurnos.WebApplication.WebApi.Extension
{
    public static class HttpStatusCodeExtension
    {
        public static int GetCode(this HttpStatusCode httpStatusCode)
        {
            return (int)httpStatusCode;
        }
    }
}
