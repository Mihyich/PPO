using MetroGid.Core.Interfaces;
using MCMC = MetroGid.Core.Models.Concrete;
using MCMA = MetroGid.Core.Models.Advanced;
using MetroGid.DBA.EF.Context;
using Microsoft.EntityFrameworkCore;
using MetroGid.DBA.EF.Converters;

namespace MetroGid.DBA.EF.Repositories;

public class EFRouteRepository(MetroDbContext context) : IRouteRepository
{
    private readonly MetroDbContext _context = context;

    public async Task<MCMA.IdRow> AddAsync(int clientId, int chartId, MCMC.Route route) =>
        new (await _context.Clients
            .AsNoTracking()
            .Where(c => c.Id == clientId)
            .Select(c => _context.AddRouteJson(c.Id, chartId, DomainRouteJsonConverter.Convert(route)))
            .FirstOrDefaultAsync()
        );

    public async Task<MCMA.IdRow> GetIdAsync(string title, int clientId) =>
        new (await _context.Ways
            .AsNoTracking()
            .Where(w => w.ClientId == clientId && w.Title == title)
            .Select(w => w.Id)
            .FirstOrDefaultAsync()
        );

    public async Task<MCMC.Route?> GetByIdAsync(int id)
    {
        string? routeJson = await _context.Ways
            .AsNoTracking()
            .Where(w => w.Id == id)
            .Select(w => _context.GetRouteJsonById(w.Id))
            .FirstOrDefaultAsync();

        return routeJson != null ? DomainRouteJsonConverter.Convert(routeJson) : null;
    }

    public async Task<MCMA.TitlesRow> GetAllTitlesForClientAsync(int clientId) =>
        new (await _context.Ways
            .AsNoTracking()
            .Where(w => w.ClientId == clientId)
            .Select(w => w.Title)
            .ToListAsync()
        );

    public async Task<MCMA.TitlesRow> GetAllTitlesForClientOfChartAsync(int clientId, int chartId) =>
        new (await _context.Ways
            .AsNoTracking()
            .Where(w => w.ClientId == clientId && w.ChartId == chartId)
            .Select(w => w.Title)
            .ToListAsync()
        );

    public async Task<MCMA.TitlesRow> GetAllForClientIdAsync(int clientId) =>
        new (await _context.Ways
            .AsNoTracking()
            .Where(w => w.ClientId == clientId)
            .Select(w => _context.GetRouteJsonById(w.Id))
            .ToListAsync()
        );

    public async Task<MCMA.TitlesRow> GetAllForClientOfChartIdAsync(int clientId, int chartId) =>
        new (await _context.Ways
            .AsNoTracking()
            .Where(w => w.ChartId == chartId && w.ClientId == clientId)
            .Select(w => w.Title)
            .ToListAsync()
        );

    public async Task<MCMC.Route?> GetChartRouteOfClient(
        int clientId,
        int chartId,
        string title
    )
    {
        string? routeJson = await _context.Ways
            .Where(w => w.ChartId == chartId && w.ClientId == clientId && w.Title == title)
            .Select(w => _context.GetRouteJsonById(w.Id))
            .FirstOrDefaultAsync();

        return routeJson != null ? DomainRouteJsonConverter.Convert(routeJson) : null;
    }

    public Task<MCMA.ChangedRowCount> UpdateAsync(int clientId, int chartId, MCMC.Route route)
    {
        throw new NotImplementedException();
    }

    public async Task<MCMA.DeletedRowCount> DeleteAsync(int id) =>
        new (await _context.Ways
            .Where(w => w.Id == id)
            .ExecuteDeleteAsync()
        );

    public async Task<bool> IsTitleExistsAsync(string title, int clientId) =>
        await _context.Ways
            .AsNoTracking()
            .Where(w => w.ClientId == clientId)
            .AnyAsync(w => w.Title == title);
}