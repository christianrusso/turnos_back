using Microsoft.AspNetCore.Mvc.Filters;
using SistemaTurnos.WebApplication.WebApi.Exceptions;

namespace SistemaTurnos.WebApplication.WebApi.Filters
{
    public class ModelStateValidationFilter : IActionFilter
    {
        // Do something before the action executes
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                throw new BadRequestException(ExceptionMessages.BadRequest);
            }
        }

        // Do something after the action execute
        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}
