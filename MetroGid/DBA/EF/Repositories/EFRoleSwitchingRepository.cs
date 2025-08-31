using MetroGid.Core.Interfaces;
using MetroGid.DBA.EF.Context;
using Microsoft.EntityFrameworkCore;

namespace MetroGid.DBA.EF.Repositories;

public class EFRoleSwitchingRepository(MetroDbContext context) : IRoleSwitchingRepository
{
    private readonly MetroDbContext _context = context;

    public async Task SetRoleAsync(string roleName) =>
        await _context.Database.ExecuteSqlRawAsync($"SET ROLE {roleName}");

    public async Task ResetRoleAsync() =>
        await _context.Database.ExecuteSqlRawAsync("RESET ROLE");
}