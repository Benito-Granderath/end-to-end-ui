using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RGLNR_Interface.Services;

public class ValidateSidAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var localTestingContext = context.HttpContext.RequestServices.GetService(typeof(ILocalTestingContext)) as ILocalTestingContext;

        if (localTestingContext?.IsEnabled == true)
        {
            return;
        }

        var actionName = context.ActionDescriptor.RouteValues["action"];
        if (actionName != "AccessDenied")
        {
            if (context.HttpContext.User.Identity?.IsAuthenticated == true)
            {
                var username = context.HttpContext.User.Identity.Name;
                if (string.IsNullOrEmpty(username))
                {
                    context.Result = new RedirectToActionResult("AccessDenied", "Home", null);
                }
            }
            else
            {
                context.Result = new RedirectToActionResult("AccessDenied", "Home", null);
            }
        }
    }
}
