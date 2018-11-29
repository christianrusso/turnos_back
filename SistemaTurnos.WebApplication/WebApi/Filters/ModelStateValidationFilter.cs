using Microsoft.AspNetCore.Mvc.Filters;
using SistemaTurnos.Commons.Exceptions;

namespace SistemaTurnos.WebApplication.WebApi.Filters
{
    public class ModelStateValidationFilter : IActionFilter
    {
        // Do something before the action executes
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                throw new BadRequestException();
            }
        }

        // Do something after the action execute
        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}
