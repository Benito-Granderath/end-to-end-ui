using System.Security.Claims;
using RGLNR_Interface.Services;

namespace RGLNR_Interface.Middleware
{
    public class LocalTestingUserMiddleware
    {
        private readonly RequestDelegate _next;

        public LocalTestingUserMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ILocalTestingContext localTestingContext)
        {
            if (localTestingContext.IsEnabled)
            {
                var identity = new ClaimsIdentity(
                    new[]
                    {
                        new Claim(ClaimTypes.Name, localTestingContext.UserName)
                    },
                    authenticationType: "LocalTesting");

                context.User = new ClaimsPrincipal(identity);
            }

            await _next(context);
        }
    }
}
