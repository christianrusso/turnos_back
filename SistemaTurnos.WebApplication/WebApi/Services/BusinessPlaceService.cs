using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaTurnos.WebApplication.WebApi.Services
{
    public class BusinessPlaceService:ServiceBase
    {
        public BusinessPlaceService(HttpContext httpContext) : base(httpContext) { }
    }
}
