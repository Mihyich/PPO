namespace MetroGid.Controllers.Utility.Interfaces;

public interface IRoleSwitchingService
{
    Task SwitchToRoleAsync(string roleName);
    Task ResetToDefaultRoleAsync();
}