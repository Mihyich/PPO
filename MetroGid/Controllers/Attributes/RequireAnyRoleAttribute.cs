using System.Security.Claims;
using System.Text.Json.Serialization;
using MetroGid.Controllers.Utility.Converters;
using MetroGid.Controllers.Utility.DTO.Concrete;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MetroGid.Controllers.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class RequireAnyRoleAttribute : Attribute, IAuthorizationFilter
{
    private readonly RoleTypeDTO[] _allowedRoles;

    public RequireAnyRoleAttribute(params RoleTypeDTO[] roles)
    {
        _allowedRoles = roles ?? throw new ArgumentNullException(nameof(roles));
        
        if (_allowedRoles.Length == 0)
            throw new ArgumentException(
                "Должна быть указана хотя бы одна роль",
                nameof(roles)
            );
    }
    
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        ClaimsPrincipal? user = context.HttpContext.User;
        string? roleClaim = user.FindFirst(ClaimTypes.Role)?.Value;

        if (string.IsNullOrEmpty(roleClaim))
        {
            SetUnauthorized(context);
            return;
        }


        RoleTypeDTO role = DBRoleToDTORoleConverter.Convert(roleClaim);

        if (!_allowedRoles.Contains(role))
        {
            SetForbidden(context);
            return;
        }
    }

    private static void SetUnauthorized(AuthorizationFilterContext context)
    {
        context.Result = new ObjectResult(
            new
            {
                Error = "InvalidCredentials",
                Message = "Не удалось определить роль пользователя"
            }
        )
        {
            StatusCode = StatusCodes.Status401Unauthorized
        };
    }

    private static void SetForbidden(AuthorizationFilterContext context)
    {
        context.Result = new ObjectResult(new
        {
            Error = "Forbidden",
            Message = "Доступ запрещён"
        })
        {
            StatusCode = StatusCodes.Status403Forbidden
        };
    }
}