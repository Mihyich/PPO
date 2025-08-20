using MetroGid.Core.Interfaces;
using MCMC = MetroGid.Core.Models.Concrete;
using MetroGid.DBA.EF.Context;
using Microsoft.EntityFrameworkCore;
using MetroGid.DBA.EF.Converters;

namespace MetroGid.DBA.EF.Repositories;

public class EFRouteRepository(MetroDbContext context) : IRouteRepository
{
    private readonly MetroDbContext _context = context;

    public async Task<int> AddAsync(int clientId, int chartId, MCMC.Route route) =>
        await _context.Clients
            .AsNoTracking()
            .Where(c => c.Id == clientId)
            .Select(c => _context.AddRouteJson(c.Id, chartId, DomainRouteJsonConverter.Convert(route)))
            .FirstOrDefaultAsync();

    public async Task<int> GetIdAsync(string title, int clientId) =>
        await _context.Ways
            .AsNoTracking()
            .Where(w => w.ClientId == clientId && w.Title == title)
            .Select(w => w.Id)
            .FirstOrDefaultAsync();

    public Task<MCMC.Route?> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<List<string>> GetAllTitlesForClientAsync(int clientId) =>
        await _context.Ways
            .AsNoTracking()
            .Where(w => w.ClientId == clientId)
            .Select(w => w.Title)
            .ToListAsync();

    public async Task<List<string>> GetAllTitlesForClientOfChartAsync(int clientId, int chartId) =>
        await _context.Ways
            .AsNoTracking()
            .Where(w => w.ClientId == clientId && w.ChartId == chartId)
            .Select(w => w.Title)
            .ToListAsync();

    public Task<List<MCMC.Route>> GetAllForClientIdAsync(int clientId)
    {
        throw new NotImplementedException();
    }

    public Task<List<MCMC.Route>> GetAllForClientOfChartIdAsync(int clientId, int chartId)
    {
        throw new NotImplementedException();
    }

    public Task<int> UpdateAsync(int clientId, int chartId, MCMC.Route route)
    {
        throw new NotImplementedException();
    }

    public async Task<int> DeleteAsync(int id) =>
        await _context.Ways
            .Where(w => w.Id == id)
            .ExecuteDeleteAsync();

    public async Task<bool> IsTitleExistsAsync(string title, int clientId) =>
        await _context.Ways
            .Where(w => w.ClientId == clientId)
            .AnyAsync(w => w.Title == title);
}