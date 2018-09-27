using Microsoft.AspNetCore.Http;
using SistemaTurnos.WebApplication.WebApi.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaTurnos.WebApplication.WebApi.Services
{
    public class ServiceBase
    {
        protected readonly HttpContext _httpContext;
        public ServiceBase(HttpContext httpContext)
        {
            _httpContext = httpContext;
        }

        public int GetUserId()
        {
            /*int? userId = (int?)_httpContext.Items["userId"];

            if (!userId.HasValue)
            {
                throw new ApplicationException(ExceptionMessages.InternalServerError);
            }

            return userId.Value;*/
            return 26;
        }
    }
}
