using MetroGid.Core.Interfaces;
using MCMC = MetroGid.Core.Models.Concrete;
using MDEMT = MetroGid.DBA.EF.Models.Tables;
using MetroGid.DBA.EF.Context;
using Microsoft.EntityFrameworkCore;
using MetroGid.DBA.EF.Converters;
using MetroGid.DBA.EF.Models.Shadow;

namespace MetroGid.DBA.EF.Repositories;

public class EFChartRepository(MetroDbContext context) : IChartRepository
{
    private readonly MetroDbContext _context = context;

    public async Task<int> AddAsync(string chartJson)
    {
        var result = await _context.Set<ScalarResult>()
            .FromSqlRaw("SELECT public.add_chart_json({0}) AS \"Value\" FROM (VALUES (1)) AS fake", chartJson)
            .AsAsyncEnumerable()
            .FirstOrDefaultAsync();

        return result?.Value ?? 0;
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

    public async Task<int> GetRailwayIdAsync(int stationId1, int stationId2)
    {
        ValueTuple<int, int>? neighborTransitionIds1 = await GetNeighborStationRailwayIdAsync(stationId1);
        ValueTuple<int, int>? neighborTransitionIds2 = await GetNeighborStationRailwayIdAsync(stationId2);

        if (neighborTransitionIds1 != null && neighborTransitionIds2 != null)
        {
            List<int> Ids1 = [neighborTransitionIds1.Value.Item1, neighborTransitionIds1.Value.Item2];
            List<int> Ids2 = [neighborTransitionIds2.Value.Item1, neighborTransitionIds2.Value.Item2];

            return Ids1.Intersect(Ids2).Where(id => id > 0).FirstOrDefault();
        }

        return 0;
    }

    public async Task<int> GetTransitionIdAsync(int stationId1, int stationId2)
    {
        List<int> neighborTransitionIds1 = await GetNeighborStationTransitionIdAsync(stationId1);
        List<int> neighborTransitionIds2 = await GetNeighborStationTransitionIdAsync(stationId2);
        return neighborTransitionIds1.Intersect(neighborTransitionIds2).FirstOrDefault();
    }

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

    public async Task<string?> GetChartJsonByIdAsync(int chartId) =>
        await _context.Charts
            .AsNoTracking()
            .Where(c => c.Id == chartId)
            .Select(c => _context.GetChartJsonById(c.Id))
            .FirstOrDefaultAsync();

    public async Task<MCMC.Chart?> GetChartWeakByIdAsync(int chartId)
    {
        MDEMT.Chart? chart = await _context.Charts
            .AsNoTracking()
            .Where(c => c.Id == chartId)
            .FirstOrDefaultAsync();

        return chart != null ? ModelDomainConverter.Convert(chart) : null;
    }

    public async Task<string?> GetChartSchemeByIdAsync(int chartId) =>
        await _context.Charts
            .AsNoTracking()
            .Where(c => c.Id == chartId)
            .Select(c => c.SvgContent)
            .FirstOrDefaultAsync();

    public async Task<MCMC.Branch?> GetBranchWeakByIdAsync(int branchId)
    {
        MDEMT.Branch? branch = await _context.Branches
            .AsNoTracking()
            .Where(b => b.Id == branchId)
            .FirstOrDefaultAsync();

        return branch != null ? ModelDomainConverter.Convert(branch) : null;
    }

    public async Task<MCMC.Station?> GetStationWeakByIdAsync(int stationId)
    {
        MDEMT.Station? station = await _context.Stations
            .AsNoTracking()
            .Where(s => s.Id == stationId)
            .FirstOrDefaultAsync();

        return station != null ? ModelDomainConverter.Convert(station) : null;
    }

    public async Task<MCMC.Railway?> GetRailwayByIdAsync(int railwayId)
    {
        MDEMT.Railway? railway = await _context.Railways
            .AsNoTracking()
            .Where(r => r.Id == railwayId)
            .FirstOrDefaultAsync();

        return railway != null ? ModelDomainConverter.Convert(railway) : null;
    }

    public async Task<MCMC.Transition?> GetTransitionByIdAsync(int transitionId)
    {
        MDEMT.Transition? transition = await _context.Transitions
            .AsNoTracking()
            .Where(t => t.Id == transitionId)
            .FirstOrDefaultAsync();

        return transition != null ? ModelDomainConverter.Convert(transition) : null;
    }

    public async Task<string?> GetChartJsonByCredentialsAsync(string city, string title) =>
        await _context.Charts
            .AsNoTracking()
            .Where(c => c.City == city && c.Title == title)
            .Select(c => _context.GetChartJsonById(c.Id))
            .FirstOrDefaultAsync();

    public async Task<List<(string, string)>> GetAllChartCityTitleAsync() =>
        (await _context.Charts
            .AsNoTracking()
            .Select(c => new { c.City, c.Title })
            .ToListAsync())
                .Select(x => (x.City, x.Title))
                .ToList();

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

    public async Task<(int, int)?> GetNeighborStationRailwayIdAsync(int stationId)
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

        return (fromId > 0 && toId > 0) ? (fromId, toId) : null;
    }

    public async Task<List<int>> GetNeighborStationTransitionIdAsync(int stationId) =>
        await _context.StationTransitions
            .AsNoTracking()
            .Where(st => st.StationId == stationId)
            .Select(st => st.TransitionId)
            .ToListAsync();

    public async Task<(int, int)?> GetFromToStationIdByRailwayIdAsync(int railwayId)
    {
        var result = await _context.Railways
            .AsNoTracking()
            .Where(r => r.Id == railwayId)
            .Select(r => new { r.FromId, r.ToId })
            .FirstOrDefaultAsync();

        return result != null ? (result.FromId, result.ToId) : null;
    }

    public async Task<(int, int)?> GetFromToStationIdByTransitionIdAsync(int transitionId)
    {
        var result = await _context.StationTransitions
            .AsNoTracking()
            .Where(t => t.TransitionId == transitionId)
            .Select(st => st.StationId)
            .ToListAsync();

        return result.Count == 2 ? (result[0], result[1]) : null;
    }

    public async Task<int> UpdateChartByIdAsync(int chartId, MCMC.Chart chart)
    {
        MDEMT.Chart? trackEntity = await _context.Charts.FirstOrDefaultAsync(c => c.Id == chartId);

        if (trackEntity != null)
        {
            MDEMT.Chart updChart = DomainModelConverter.Convert(chart);

            trackEntity.City = updChart.City;
            trackEntity.Title = updChart.Title;
        }

        return await _context.SaveChangesAsync();
    }

    public async Task<int> UpdateChartSchemeByIdAsync(int chartId, string scheme)
    {
        MDEMT.Chart? trackEntity = await _context.Charts.FirstOrDefaultAsync(c => c.Id == chartId);

        if (trackEntity != null)
            trackEntity.SvgContent = scheme;

        return await _context.SaveChangesAsync();
    }

    public async Task<int> UpdateBranchByIdAsync(int branchId, MCMC.Branch branch)
    {
        MDEMT.Branch? trackEntity = await _context.Branches.FirstOrDefaultAsync(b => b.Id == branchId);

        if (trackEntity != null)
        {
            MDEMT.Branch updBranch = DomainModelConverter.Convert(branch);

            trackEntity.Title = updBranch.Title;
            trackEntity.Color = updBranch.Color;
            trackEntity.Access = updBranch.Access;
        }

        return await _context.SaveChangesAsync();
    }

    public async Task<int> UpdateStationByIdAsync(int stationId, MCMC.Station station)
    {
        MDEMT.Station? trackEntity = await _context.Stations.FirstOrDefaultAsync(s => s.Id == stationId);

        if (trackEntity != null)
        {
            MDEMT.Station updStation = DomainModelConverter.Convert(station);

            trackEntity.Title = updStation.Title;
            trackEntity.Occupancy = updStation.Occupancy;
            trackEntity.Access = updStation.Access;
            trackEntity.OpenTime = updStation.OpenTime;
            trackEntity.CloseTime = updStation.CloseTime;
        }

        return await _context.SaveChangesAsync();
    }

    public async Task<int> UpdateRailwayByIdAsync(int railwayId, MCMC.Railway railway)
    {
        MDEMT.Railway? trackEntity = await _context.Railways.FirstOrDefaultAsync(r => r.Id == railwayId);

        if (trackEntity != null)
        {
            MDEMT.Railway updRailway = DomainModelConverter.Convert(railway);

            trackEntity.Duration = updRailway.Duration;
        }

        return await _context.SaveChangesAsync();
    }

    public async Task<int> UpdateTransitionByIdAsync(int transitionId, MCMC.Transition transition)
    {
        MDEMT.Transition? trackEntity = await _context.Transitions.FirstOrDefaultAsync(t => t.Id == transitionId);

        if (trackEntity != null)
        {
            MDEMT.Transition updTransition = DomainModelConverter.Convert(transition);

            trackEntity.Occupancy = updTransition.Occupancy;
            trackEntity.Access = updTransition.Access;
            trackEntity.Duration = updTransition.Duration;
            trackEntity.OpenTime = updTransition.OpenTime;
            trackEntity.CloseTime = updTransition.CloseTime;
        }

        return await _context.SaveChangesAsync();
    }

    public async Task<int> DeleteChartByIdAsync(int id) =>
        await _context.Charts
            .Where(c => c.Id == id)
            .ExecuteDeleteAsync();

    public async Task<bool> IsCityTitleUniqueAsync(string city, string title) =>
        !await _context.Charts
            .AsNoTracking()
            .Where(c => c.City == city && c.Title == title)
            .AnyAsync();

    public async Task<bool> IsBranchTitleUniqueInChartAsync(string title, int chartId) =>
        !await _context.ChartBranches
            .AsNoTracking()
            .Where(cb => cb.ChartId == chartId)
            .AnyAsync(cb => cb.Branch.Title == title);
}