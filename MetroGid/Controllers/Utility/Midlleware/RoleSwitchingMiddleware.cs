using System.Security;
using System.Security.Claims;
using MetroGid.Controllers.Utility.Interfaces;

namespace MetroGid.Controllers.Utility.Middleware;

public class RoleSwitchingMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context, IRoleSwitchingService roleSwitchingService)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            string? role = context.User.FindFirst(ClaimTypes.Role)?.Value;

            if (!string.IsNullOrEmpty(role))
            {
                await roleSwitchingService.SwitchToRoleAsync(role);
            }
            else
            {
                await roleSwitchingService.ResetToDefaultRoleAsync();
            }
        }
        else
        {
            await roleSwitchingService.ResetToDefaultRoleAsync();
        }

        await _next(context);
    }
}