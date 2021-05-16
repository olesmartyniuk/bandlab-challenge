using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Imagegram.Api.Controllers
{
    public class ModelStateValidator : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext actionExecutingContext)
        {
            if (!actionExecutingContext.ModelState.IsValid)
            {
                actionExecutingContext.Result = new BadRequestObjectResult(actionExecutingContext.ModelState);
            }
        }
    }

}
