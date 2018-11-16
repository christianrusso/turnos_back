using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SistemaTurnos.WebApplication.WebApi.Extension;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace SistemaTurnos.WebApplication.WebApi.Exceptions
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<ExceptionHandlerMiddleware> logger;

        public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception exception)
            {
                if (!(exception is BadRequestException))
                {
                    logger.LogError(exception, $"User ID: {context.Items["userId"] ?? "Anonimo" } | Request Body {GetRequestBody(context)}");
                }

                await HandleExceptionAsync(context, exception);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = exception is BadRequestException ? HttpStatusCode.BadRequest.GetCode() : HttpStatusCode.InternalServerError.GetCode();
            return context.Response.WriteAsync(exception.Message);
        }

        private string GetRequestBody(HttpContext context)
        {
            context.Request.Body.Seek(0, SeekOrigin.Begin);

            using (var reader = new StreamReader(context.Request.Body))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
