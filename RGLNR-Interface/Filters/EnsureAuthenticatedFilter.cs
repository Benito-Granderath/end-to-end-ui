using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RGLNR_Interface.Services;

namespace RGLNR_Interface.Filters
{
    public class EnsureAuthenticatedFilter : IActionFilter
    {
        private readonly ILocalTestingContext _localTestingContext;

        public EnsureAuthenticatedFilter(ILocalTestingContext localTestingContext)
        {
            _localTestingContext = localTestingContext;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (_localTestingContext.IsEnabled)
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

        public void OnActionExecuted(ActionExecutedContext context) { }
    }
}
