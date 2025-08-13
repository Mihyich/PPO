using MetroGid.Core.Interfaces;
using MCMC = MetroGid.Core.Models.Concrete;
using MetroGid.DBA.EF.Models.Tables;
using MetroGid.DBA.EF.Converters;
using MetroGid.DBA.EF.Context;
using Microsoft.EntityFrameworkCore;

namespace MetroGid.DBA.EF.Repositories;

public class EFRouteRepository(MetroDbContext context) : IRouteRepository
{
    private readonly MetroDbContext _context = context;

    public async Task<int> AddAsync(MCMC.Route route, int clientId, int chartId)
    {
        throw new NotImplementedException();
    }

    public async Task<int> GetIdAsync(string title, int clientId)
    {
        throw new NotImplementedException();
    }

    public async Task<MCMC.Route?> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<List<string>?> GetAllTitlesForClientAsync(int clientId)
    {
        throw new NotImplementedException();
    }

    public async Task<List<string>?> GetAllTitlesForClientOfChartAsync(int clientId, int chartId)
    {
        throw new NotImplementedException();
    }

    public async Task<List<MCMC.Route>?> GetAllForClientIdAsync(int clientId)
    {
        throw new NotImplementedException();
    }

    public async Task<List<MCMC.Route>?> GetAllForClientOfChartIdAsync(int clientId, int chartId)
    {
        throw new NotImplementedException();
    }

    public async Task UpdateAsync(int id, MCMC.Route route)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> IsTitleExistsAsync(string title, int clientId)
    {
        throw new NotImplementedException();
    }
}