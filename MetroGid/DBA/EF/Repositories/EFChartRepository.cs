using MetroGid.Core.Interfaces;
using MCMC = MetroGid.Core.Models.Concrete;
using MCMA = MetroGid.Core.Models.Advanced;
using MDEMT = MetroGid.DBA.EF.Models.Tables;
using MetroGid.DBA.EF.Context;
using Microsoft.EntityFrameworkCore;
using MetroGid.DBA.EF.Converters;
using MetroGid.DBA.EF.Models.Shadow;

namespace MetroGid.DBA.EF.Repositories;

public class EFChartRepository(MetroDbContext context) : IChartRepository
{
    private readonly MetroDbContext _context = context;

    public async Task<MCMA.IdRow> AddAsync(string chartJson)
    {
        var result = await _context.Set<ScalarResult>()
            .FromSqlRaw("SELECT public.add_chart_json({0}) AS \"Value\" FROM (VALUES (1)) AS fake", chartJson)
            .AsAsyncEnumerable()
            .FirstOrDefaultAsync();

        return new(result?.Value ?? 0);
    }

    public async Task<MCMA.IdRow> GetChartIdAsync(string city, string title) =>
        new (await _context.Charts
            .AsNoTracking()
            .Where(c => c.City == city && c.Title == title)
            .Select(c => c.Id)
            .FirstOrDefaultAsync()
        );

    public async Task<MCMA.IdRow> GetBranchIdAsync(string title, int chartId) =>
        new(await _context.ChartBranches
            .AsNoTracking()
            .Where(cb => cb.ChartId == chartId)
            .Select(cb => cb.Branch)
            .Where(b => b.Title == title)
            .Select(b => b.Id)
            .FirstOrDefaultAsync()
        );

    public async Task<MCMA.IdRow> GetStationIdAsync(string title, int branchId) =>
        new(await _context.BranchStations
            .AsNoTracking()
            .Where(bs => bs.BranchId == branchId)
            .Select(bs => bs.Station)
            .Where(s => s.Title == title)
            .Select(s => s.Id)
            .FirstOrDefaultAsync()
        );

    public async Task<MCMA.IdRow> GetRailwayIdAsync(int stationId1, int stationId2)
    {
        MCMA.IdNexusRow? neighborTransitionIds1 = await GetNeighborStationRailwayIdAsync(stationId1);
        MCMA.IdNexusRow? neighborTransitionIds2 = await GetNeighborStationRailwayIdAsync(stationId2);

        if (neighborTransitionIds1 != null && neighborTransitionIds2 != null)
        {
            List<int> Ids1 = [neighborTransitionIds1.FromId, neighborTransitionIds1.ToId];
            List<int> Ids2 = [neighborTransitionIds2.FromId, neighborTransitionIds2.ToId];

            return new (Ids1.Intersect(Ids2).Where(id => id > 0).FirstOrDefault());
        }

        return new (0);
    }

    public async Task<MCMA.IdRow> GetTransitionIdAsync(int stationId1, int stationId2)
    {
        MCMA.IdsRow neighborTransitionIds1 = await GetNeighborStationTransitionIdAsync(stationId1);
        MCMA.IdsRow neighborTransitionIds2 = await GetNeighborStationTransitionIdAsync(stationId2);
        return new (neighborTransitionIds1.ids.Intersect(neighborTransitionIds2.ids).FirstOrDefault());
    }

    public async Task<MCMA.IdsRow> GetAllChartIdAsync() =>
        new (await _context.Charts
            .AsNoTracking()
            .Select(c => c.Id)
            .ToListAsync()
        );

    public async Task<MCMA.IdsRow> GetAllChartBranchIdAsync(int chartId) =>
        new (await _context.ChartBranches
            .AsNoTracking()
            .Where(cb => cb.ChartId == chartId)
            .Select(cb => cb.BranchId)
            .ToListAsync()
        );

    public async Task<MCMA.IdsRow> GetAllBranchStationIdAsync(int branchId) =>
        new (await _context.BranchStations
            .AsNoTracking()
            .Where(bs => bs.BranchId == branchId)
            .Select(bs => bs.StationId)
            .ToListAsync()
        );

    public async Task<MCMA.FileRow?> GetChartJsonByIdAsync(int chartId)
    {
        string? content = await _context.Charts
            .AsNoTracking()
            .Where(c => c.Id == chartId)
            .Select(c => _context.GetChartJsonById(c.Id))
            .FirstOrDefaultAsync();

        return content != null ? new(content) : null;
    }

    public async Task<MCMC.Chart?> GetChartWeakByIdAsync(int chartId)
    {
        MDEMT.Chart? chart = await _context.Charts
            .AsNoTracking()
            .Where(c => c.Id == chartId)
            .FirstOrDefaultAsync();

        return chart != null ? ModelDomainConverter.Convert(chart) : null;
    }

    public async Task<MCMA.FileRow?> GetChartSchemeByIdAsync(int chartId)
    {
        string? content = await _context.Charts
            .AsNoTracking()
            .Where(c => c.Id == chartId)
            .Select(c => c.SvgContent)
            .FirstOrDefaultAsync();

        return content != null ? new(content) : null;
    }

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

    public async Task<MCMA.FileRow?> GetChartJsonByCredentialsAsync(string city, string title)
    {
        string? content = await _context.Charts
            .AsNoTracking()
            .Where(c => c.City == city && c.Title == title)
            .Select(c => _context.GetChartJsonById(c.Id))
            .FirstOrDefaultAsync();
        
        return content != null ? new(content) : null;
    }

    public async Task<MCMA.ChartIdentifiers> GetAllChartCityTitleAsync() =>
        new(await _context.Charts
            .AsNoTracking()
            .Select(c => new MCMA.ChartIdentifier(c.City, c.Title))
            .ToListAsync()
        );

    public async Task<MCMA.TitlesRow> GetAllChartBranchTitleAsync(int chartId) =>
        new(await _context.ChartBranches
            .AsNoTracking()
            .Where(cb => cb.ChartId == chartId)
            .Select(cb => cb.Branch)
            .Select(b => b.Title)
            .ToListAsync()
        );

    public async Task<MCMA.TitlesRow> GetAllBranchStationTitleAsync(int branchId) =>
        new(await _context.BranchStations
            .AsNoTracking()
            .Where(bs => bs.BranchId == branchId)
            .Select(bs => bs.Station)
            .Select(s => s.Title)
            .ToListAsync()
        );

    public async Task<MCMA.IdNexusRow?> GetNeighborStationRailwayIdAsync(int stationId)
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

        return (fromId > 0 && toId > 0) ? new(fromId, toId) : null;
    }

    public async Task<MCMA.IdsRow> GetNeighborStationTransitionIdAsync(int stationId) =>
        new (await _context.StationTransitions
            .AsNoTracking()
            .Where(st => st.StationId == stationId)
            .Select(st => st.TransitionId)
            .ToListAsync()
        );

    public async Task<MCMA.IdNexusRow?> GetFromToStationIdByRailwayIdAsync(int railwayId) =>
        await _context.Railways
            .AsNoTracking()
            .Where(r => r.Id == railwayId)
            .Select(r => new MCMA.IdNexusRow(r.FromId, r.ToId))
            .FirstOrDefaultAsync();

    public async Task<MCMA.IdNexusRow?> GetFromToStationIdByTransitionIdAsync(int transitionId)
    {
        var result = await _context.StationTransitions
            .AsNoTracking()
            .Where(t => t.TransitionId == transitionId)
            .Select(st => st.StationId)
            .ToListAsync();

        return result.Count == 2 ? new(result[0], result[1]) : null;
    }

    public async Task<MCMA.IdRow?> GetStationDutyIdAsync(int stationId)
    {
        int? dutyId = await _context.Stations
            .AsNoTracking()
            .Where(s => s.Id == stationId)
            .Select(s => s.DutyId)
            .FirstOrDefaultAsync();

        return dutyId != null ? new(dutyId ?? 0) : null;
    }

    public async Task<MCMA.IdRow?> GetTransitionDutyIdAsync(int transitionId)
    {
        int? dutyId = await _context.Transitions
            .AsNoTracking()
            .Where(t => t.Id == transitionId)
            .Select(t => t.DutyId)
            .FirstOrDefaultAsync();

        return dutyId != null ? new(dutyId ?? 0) : null;
    }

    public async Task<MCMA.ChangedRowCount> UpdateChartByIdAsync(int chartId, MCMC.Chart chart)
    {
        MDEMT.Chart? trackEntity = await _context.Charts.FirstOrDefaultAsync(c => c.Id == chartId);

        if (trackEntity != null)
        {
            MDEMT.Chart updChart = DomainModelConverter.Convert(chart);

            trackEntity.City = updChart.City;
            trackEntity.Title = updChart.Title;
        }

        return new(await _context.SaveChangesAsync());
    }

    public async Task<MCMA.ChangedRowCount> UpdateChartSchemeByIdAsync(int chartId, string scheme)
    {
        MDEMT.Chart? trackEntity = await _context.Charts.FirstOrDefaultAsync(c => c.Id == chartId);

        if (trackEntity != null)
            trackEntity.SvgContent = scheme;

        return new(await _context.SaveChangesAsync());
    }

    public async Task<MCMA.ChangedRowCount> UpdateBranchByIdAsync(int branchId, MCMC.Branch branch)
    {
        MDEMT.Branch? trackEntity = await _context.Branches.FirstOrDefaultAsync(b => b.Id == branchId);

        if (trackEntity != null)
        {
            MDEMT.Branch updBranch = DomainModelConverter.Convert(branch);

            trackEntity.Title = updBranch.Title;
            trackEntity.Color = updBranch.Color;
            trackEntity.Access = updBranch.Access;
        }

        return new(await _context.SaveChangesAsync());
    }

    public async Task<MCMA.ChangedRowCount> UpdateStationByIdAsync(int stationId, MCMC.Station station)
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

        return new(await _context.SaveChangesAsync());
    }

    public async Task<MCMA.ChangedRowCount> UpdateRailwayByIdAsync(int railwayId, MCMC.Railway railway)
    {
        MDEMT.Railway? trackEntity = await _context.Railways.FirstOrDefaultAsync(r => r.Id == railwayId);

        if (trackEntity != null)
        {
            MDEMT.Railway updRailway = DomainModelConverter.Convert(railway);

            trackEntity.Duration = updRailway.Duration;
        }

        return new(await _context.SaveChangesAsync());
    }

    public async Task<MCMA.ChangedRowCount> UpdateTransitionByIdAsync(int transitionId, MCMC.Transition transition)
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

        return new(await _context.SaveChangesAsync());
    }

    public async Task<MCMA.DeletedRowCount> DeleteChartByIdAsync(int id) =>
        new (await _context.Charts
            .Where(c => c.Id == id)
            .ExecuteDeleteAsync()
        );

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