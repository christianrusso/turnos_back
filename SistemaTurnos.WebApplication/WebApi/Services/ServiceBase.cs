using Microsoft.AspNetCore.Http;
using SistemaTurnos.Commons.Exceptions;
using System;

namespace SistemaTurnos.WebApplication.WebApi.Services
{
    public class ServiceBase
    {
        public int GetUserId(HttpContext httpContext)
        {
            int? userId = (int?)httpContext.Items["userId"];

            if (!userId.HasValue)
            {
                throw new ApplicationException(ExceptionMessages.InternalServerError);
            }

            return userId.Value;
        }

        public int? GetUserIdOrDefault(HttpContext httpContext)
        {
            return (int?)httpContext.Items["userId"];
        }
    }
}
