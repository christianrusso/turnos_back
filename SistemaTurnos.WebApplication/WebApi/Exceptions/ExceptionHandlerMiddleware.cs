using Microsoft.AspNetCore.Http;
using SistemaTurnos.WebApplication.WebApi.Extension;
using System;
using System.Net;
using System.Threading.Tasks;

namespace SistemaTurnos.WebApplication.WebApi.Exceptions
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate next;

        public ExceptionHandlerMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception exception)
            {
                await HandleExceptionAsync(context, exception);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = exception is BadRequestException ? HttpStatusCode.BadRequest.GetCode() : HttpStatusCode.InternalServerError.GetCode();
            // var result = JsonConvert.SerializeObject(new { error = exception.Message });
            return context.Response.WriteAsync(exception.Message);
        }
    }
}
