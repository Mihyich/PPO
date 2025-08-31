using System.Security;
using MetroGid.Controllers.Utility.Configuration;
using MetroGid.Controllers.Utility.Interfaces;
using MetroGid.Core.Interfaces;
using Microsoft.Extensions.Options;

namespace MetroGid.Core.Services;

public class RoleSwitchingService(
    IRoleSwitchingRepository roleSwitchingRepository,
    IOptions<AppRolesConfig> appRolesOptions) : IRoleSwitchingService
{
    private readonly IRoleSwitchingRepository _roleSwitchingRepository = roleSwitchingRepository;
    private readonly AppRolesConfig _appRolesOptions = appRolesOptions.Value;

    public async Task SwitchToRoleAsync(string roleName)
    {
        if (!_appRolesOptions.Roles.Contains(roleName))
            throw new SecurityException(
                $"Роль '{roleName}' не поддерживается. Допустимые роли: {string.Join(", ", _appRolesOptions.Roles)}"
            );

        await _roleSwitchingRepository.SetRoleAsync(roleName);
    }

    public async Task ResetToDefaultRoleAsync() =>
        await _roleSwitchingRepository.ResetRoleAsync();
}