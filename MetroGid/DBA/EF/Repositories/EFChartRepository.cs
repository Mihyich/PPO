using MetroGid.Core.Interfaces;
using MCMC = MetroGid.Core.Models.Concrete;
using MetroGid.DBA.EF.Models.Tables;
using MetroGid.DBA.EF.Context;
using Microsoft.EntityFrameworkCore;
using MetroGid.DBA.EF.Converters;

namespace MetroGid.DBA.EF.Repositories;

public class EFChartRepository(MetroDbContext context) : IChartRepository
{
    private readonly MetroDbContext _context = context;

    public Task<int> AddAsync(MCMC.Chart chart)
    {
        throw new NotImplementedException();
    }

    public async Task<string> GetChartJsonByIdAsync(int chartId) =>
        await _context.Charts
            .AsNoTracking()
            .Where(c => c.Id == chartId)
            .Select(c => _context.GetChartJsonById(c.Id))
            .FirstOrDefaultAsync()
            ?? throw new Exception();

    public async Task<string> GetChartJsonAsync(string city, string title) =>
        await _context.Charts
            .AsNoTracking()
            .Where(c => c.City == city && c.Title == title)
            .Select(c => _context.GetChartJsonById(c.Id))
            .FirstOrDefaultAsync()
            ?? throw new Exception();

    public async Task<MCMC.Chart> GetChartWeakByIdAsync(int chartId)
    {
        Chart chart = await _context.Charts
            .AsNoTracking()
            .Where(c => c.Id == chartId)
            .FirstOrDefaultAsync()
            ?? throw new Exception();

        return ModelDomainConverter.Convert(chart);
    }

    public async Task<MCMC.Branch> GetBranchWeakByIdAsync(int branchId)
    {
        Branch branch = await _context.Branches
            .AsNoTracking()
            .Where(b => b.Id == branchId)
            .FirstOrDefaultAsync()
            ?? throw new Exception();

        return ModelDomainConverter.Convert(branch);
    }

    public async Task<MCMC.Station> GetStationWeakByIdAsync(int stationId)
    {
        Station station = await _context.Stations
            .AsNoTracking()
            .Where(s => s.Id == stationId)
            .FirstOrDefaultAsync()
            ?? throw new Exception();

        return ModelDomainConverter.Convert(station);
    }

    public async Task<MCMC.Railway> GetRailwayByIdAsync(int railwayId)
    {
        Railway railway = await _context.Railways
            .AsNoTracking()
            .Where(r => r.Id == railwayId)
            .FirstOrDefaultAsync()
            ?? throw new Exception();

        return ModelDomainConverter.Convert(railway);
    }

    public async Task<MCMC.Transition> GetTransitionByIdAsync(int transitionId)
    {
        Transition transition = await _context.Transitions
            .AsNoTracking()
            .Where(t => t.Id == transitionId)
            .FirstOrDefaultAsync()
            ?? throw new Exception();

        return ModelDomainConverter.Convert(transition);
    }

    public async Task<int> GetChartIdAsync(string city, string title) =>
        await _context.Charts
            .AsNoTracking()
            .Where(c => c.City == city && c.Title == title)
            .Select(c => c.Id)
            .FirstOrDefaultAsync();

    public async Task<int> GetBranchIdAsync(string title, int chartId) =>
        await _context.ChartBranches
            .AsNoTracking()
            .Where(cb => cb.ChartId == chartId)
            .Select(cb => cb.Branch)
            .Where(b => b.Title == title)
            .Select(b => b.Id)
            .FirstOrDefaultAsync();

    public async Task<int> GetStationIdAsync(string title, int branchId) =>
        await _context.BranchStations
            .AsNoTracking()
            .Where(bs => bs.BranchId == branchId)
            .Select(bs => bs.Station)
            .Where(s => s.Title == title)
            .Select(s => s.Id)
            .FirstOrDefaultAsync();

    public async Task<List<int>> GetAllChartIdAsync() =>
        await _context.Charts
            .AsNoTracking()
            .Select(c => c.Id)
            .ToListAsync();

    public async Task<List<int>> GetAllChartBranchIdAsync(int chartId) =>
        await _context.ChartBranches
            .AsNoTracking()
            .Where(cb => cb.ChartId == chartId)
            .Select(cb => cb.BranchId)
            .ToListAsync();
    
    public async Task<List<int>> GetAllBranchStationIdAsync(int branchId) =>
        await _context.BranchStations
            .AsNoTracking()
            .Where(bs => bs.BranchId == branchId)
            .Select(bs => bs.StationId)
            .ToListAsync();

    public async Task<(string, string)> GetAllChartCityTitleAsync()
    {
        var query = await _context.Charts
            .AsNoTracking()
            .Select(c => new { c.City, c.Title })
            .FirstOrDefaultAsync()
            ?? throw new Exception();

        return (query.City, query.Title);
    }

    public async Task<List<string>> GetAllChartBranchTitleAsync(int chartId) =>
        await _context.ChartBranches
            .AsNoTracking()
            .Where(cb => cb.ChartId == chartId)
            .Select(cb => cb.Branch)
            .Select(b => b.Title)
            .ToListAsync();

    public async Task<List<string>> GetAllBranchStationTitleAsync(int branchId) =>
        await _context.BranchStations
            .AsNoTracking()
            .Where(bs => bs.BranchId == branchId)
            .Select(bs => bs.Station)
            .Select(s => s.Title)
            .ToListAsync();

    public async Task<(int, int)> GetNeighborStationRailwayIdAsync(int stationId)
    {
        int fromId = await _context.Railways
            .AsNoTracking()
            .Where(r => r.ToId == stationId)
            .Select(r => r.Id)
            .FirstOrDefaultAsync();

        int toId = await _context.Railways
            .AsNoTracking()
            .Where(r => r.FromId == stationId)
            .Select(r => r.Id)
            .FirstOrDefaultAsync();

        return (fromId, toId);
    }

    public async Task<List<int>> GetNeighborStationTransitionIdAsync(int stationId) =>
        await _context.StationTransitions
            .AsNoTracking()
            .Where(st => st.StationId == stationId)
            .Select(st => st.TransitionId)
            .ToListAsync();

    public async Task<(int, int)> GetFromToStationIdByRailwayIdAsync(int railwayId)
    {
        var query = await _context.Railways
            .AsNoTracking()
            .Where(r => r.Id == railwayId)
            .Select(r => new { r.FromId, r.ToId })
            .FirstOrDefaultAsync()
            ?? throw new Exception();

        return (query.FromId, query.ToId);
    }

    public async Task<(int, int)> GetFromToStationIdByTransitionIdAsync(int transitionId)
    {
        var query = await _context.StationTransitions
            .AsNoTracking()
            .Where(t => t.TransitionId == transitionId)
            .Select(st => st.StationId)
            .ToListAsync()
            ?? throw new Exception();

        if (query.Count != 2)
            throw new Exception();

        return (query[0], query[1]);
    }

    public async Task UpdateChartByIdAsync(int chartId, MCMC.Chart chart)
    {
        Chart trackEntity = await _context.Charts
            .FirstOrDefaultAsync(c => c.Id == chartId)
            ?? throw new Exception();

        Chart updatedEntity = DomainModelConverter.Convert(chart);

        _context.Entry(trackEntity).CurrentValues.SetValues(updatedEntity);
        
        await _context.SaveChangesAsync();
    }

    public async Task UpdateBranchByIdAsync(int branchId, MCMC.Branch branch)
    {
        Branch trackEntity = await _context.Branches
            .FirstOrDefaultAsync(b => b.Id == branchId)
            ?? throw new Exception();

        Branch updatedEntity = DomainModelConverter.Convert(branch);

        _context.Entry(trackEntity).CurrentValues.SetValues(updatedEntity);
        
        await _context.SaveChangesAsync();
    }

    public async Task UpdateStationByIdAsync(int stationId, MCMC.Station station)
    {
        Station trackEntity = await _context.Stations
            .FirstOrDefaultAsync(s => s.Id == stationId)
            ?? throw new Exception();

        Station updatedEntity = DomainModelConverter.Convert(station);

        _context.Entry(trackEntity).CurrentValues.SetValues(updatedEntity);
        
        await _context.SaveChangesAsync();
    }

    public async Task UpdateRailwayByIdAsync(int railwayId, MCMC.Railway railway)
    {
        Railway trackEntity = await _context.Railways
            .FirstOrDefaultAsync(r => r.Id == railwayId)
            ?? throw new Exception();

        Railway updatedEntity = DomainModelConverter.Convert(railway);

        _context.Entry(trackEntity).CurrentValues.SetValues(updatedEntity);
        
        await _context.SaveChangesAsync();
    }

    public async Task UpdateTransitionByIdAsync(int transitionId, MCMC.Transition transition)
    {
        Transition trackEntity = await _context.Transitions
            .FirstOrDefaultAsync(t => t.Id == transitionId)
            ?? throw new Exception();

        Transition updatedEntity = DomainModelConverter.Convert(transition);

        _context.Entry(trackEntity).CurrentValues.SetValues(updatedEntity);
        
        await _context.SaveChangesAsync();
    }

    public async Task DeleteChartByIdAsync(int id)
    {
        Branch branch = await _context.Branches
            .FirstOrDefaultAsync(b => b.Id == id)
            ?? throw new Exception();

        _context.Branches.Remove(branch);
        
        await _context.SaveChangesAsync();
    }
}