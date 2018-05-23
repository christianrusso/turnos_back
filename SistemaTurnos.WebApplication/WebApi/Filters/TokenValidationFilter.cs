using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using SistemaTurnos.WebApplication.WebApi.Authorization;

namespace SistemaTurnos.WebApplication.WebApi.Filters
{
    public class TokenValidationFilter : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var usingToken = context.HttpContext.Request.Headers.TryGetValue("Authorization", out StringValues token);

            if (usingToken && !ValidTokens.IsValid(token))
            {
                context.Result = new UnauthorizedResult();
            }

            if (usingToken)
            {
                context.HttpContext.Items["userId"] = ValidTokens.GetUserId(token);
            }
        }
    }
}
