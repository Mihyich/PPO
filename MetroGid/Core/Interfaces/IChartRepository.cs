using MCMC = MetroGid.Core.Models.Concrete;
using MCMA = MetroGid.Core.Models.Advanced;

namespace MetroGid.Core.Interfaces;

public interface IChartRepository
{
    Task<MCMA.IdRow> AddAsync(string chartJson);

    Task<MCMA.IdRow> GetChartIdAsync(string city, string title);
    Task<MCMA.IdRow> GetBranchIdAsync(string title, int chartId);
    Task<MCMA.IdRow> GetStationIdAsync(string title, int branchId);
    Task<MCMA.IdRow> GetRailwayIdAsync(int stationId1, int stationId2);
    Task<MCMA.IdRow> GetTransitionIdAsync(int stationId1, int stationId2);

    Task<MCMA.IdsRow> GetAllChartIdAsync();
    Task<MCMA.IdsRow> GetAllChartBranchIdAsync(int chartId);
    Task<MCMA.IdsRow> GetAllBranchStationIdAsync(int branchId);

    Task<MCMA.FileRow?> GetChartJsonByIdAsync(int chartId);
    Task<MCMC.Chart?> GetChartWeakByIdAsync(int chartId);
    Task<MCMA.FileRow?> GetChartSchemeByIdAsync(int chartId);
    Task<MCMC.Branch?> GetBranchWeakByIdAsync(int branchId);
    Task<MCMC.Station?> GetStationWeakByIdAsync(int stationId);
    Task<MCMC.Railway?> GetRailwayByIdAsync(int railwayId);
    Task<MCMC.Transition?> GetTransitionByIdAsync(int transitionId);

    Task<MCMA.FileRow?> GetChartJsonByCredentialsAsync(string city, string title);

    Task<MCMA.ChartIdentifiers> GetAllChartCityTitleAsync();
    Task<MCMA.TitlesRow> GetAllChartBranchTitleAsync(int chartId);
    Task<MCMA.TitlesRow> GetAllBranchStationTitleAsync(int branchId);

    Task<MCMA.IdNexusRow?> GetNeighborStationRailwayIdAsync(int stationId);
    Task<MCMA.IdsRow> GetNeighborStationTransitionIdAsync(int stationId);

    Task<MCMA.IdNexusRow?> GetFromToStationIdByRailwayIdAsync(int railwayId);
    Task<MCMA.IdNexusRow?> GetFromToStationIdByTransitionIdAsync(int transitionId);

    Task<MCMA.IdRow?> GetStationDutyIdAsync(int stationId);
    Task<MCMA.IdRow?> GetTransitionDutyIdAsync(int transitionId);

    Task<MCMA.ChangedRowCount> UpdateChartByIdAsync(int chartId, MCMC.Chart chart);
    Task<MCMA.ChangedRowCount> UpdateChartSchemeByIdAsync(int chartId, string scheme);
    Task<MCMA.ChangedRowCount> UpdateBranchByIdAsync(int branchId, MCMC.Branch branch);
    Task<MCMA.ChangedRowCount> UpdateStationByIdAsync(int stationId, MCMC.Station station);
    Task<MCMA.ChangedRowCount> UpdateRailwayByIdAsync(int railwayId, MCMC.Railway railway);
    Task<MCMA.ChangedRowCount> UpdateTransitionByIdAsync(int stationId, MCMC.Transition transition);

    Task<MCMA.DeletedRowCount> DeleteChartByIdAsync(int id);

    Task<bool> IsCityTitleUniqueAsync(string city, string title);
    Task<bool> IsBranchTitleUniqueInChartAsync(string title, int chartId);
}