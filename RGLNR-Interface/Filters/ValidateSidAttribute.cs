using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RGLNR_Interface.Services;
using System.Diagnostics;


public class ValidateSidAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var actionName = context.ActionDescriptor.RouteValues["action"];
        if (actionName != "AccessDenied")
        {
            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                var username = context.HttpContext.User.Identity.Name;
                if (!string.IsNullOrEmpty(username))
                {
                    var adService = context.HttpContext.RequestServices.GetService(typeof(ActiveDirectoryService)) as ActiveDirectoryService;
                    var userSID = adService?.GetUserSID(username);

                    if (string.IsNullOrEmpty(userSID))
                    {
                        context.Result = new RedirectToActionResult("AccessDenied", "Home", null);
                    }
                }
            }
            else
            {
                context.Result = new RedirectToActionResult("AccessDenied", "Home", null);
            }
        }
    }

}
