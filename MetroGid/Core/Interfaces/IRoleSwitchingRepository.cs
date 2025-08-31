namespace MetroGid.Core.Interfaces;

public interface IRoleSwitchingRepository
{
    Task SetRoleAsync(string roleName);
    Task ResetRoleAsync();
}