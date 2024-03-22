using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RGLNR_Interface.Services;

public class ValidateSidAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var adService = context.HttpContext.RequestServices.GetService(typeof(ActiveDirectoryService)) as ActiveDirectoryService;
        string username = context.HttpContext.User.Identity.Name;
        var userSID = adService?.GetUserSID(username);

        if (string.IsNullOrEmpty(userSID))
        {
            context.Result = new RedirectToActionResult("AccessDenied", "Home", null);
        }
    }
}
