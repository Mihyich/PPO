using MCMC = MetroGid.Core.Models.Concrete;

namespace MetroGid.Core.Interfaces;

public interface IChartRepository
{
    Task<int> AddAsync(string chartJson);

    Task<int> GetChartIdAsync(string city, string title);
    Task<int> GetBranchIdAsync(string title, int chartId);
    Task<int> GetStationIdAsync(string title, int branchId);
    Task<int> GetRailwayIdAsync(int stationId1, int stationId2);
    Task<int> GetTransitionIdAsync(int stationId1, int stationId2);

    Task<List<int>> GetAllChartIdAsync();
    Task<List<int>> GetAllChartBranchIdAsync(int chartId);
    Task<List<int>> GetAllBranchStationIdAsync(int branchId);

    Task<string?> GetChartJsonByIdAsync(int chartId);
    Task<MCMC.Chart?> GetChartWeakByIdAsync(int chartId);
    Task<MCMC.Branch?> GetBranchWeakByIdAsync(int branchId);
    Task<MCMC.Station?> GetStationWeakByIdAsync(int stationId);
    Task<MCMC.Railway?> GetRailwayByIdAsync(int railwayId);
    Task<MCMC.Transition?> GetTransitionByIdAsync(int transitionId);

    Task<string?> GetChartJsonByCredentialsAsync(string city, string title);

    Task<List<ValueTuple<string, string>>> GetAllChartCityTitleAsync(); // <City, Title>
    Task<List<string>> GetAllChartBranchTitleAsync(int chartId);
    Task<List<string>> GetAllBranchStationTitleAsync(int branchId);

    Task<(int, int)?> GetNeighborStationRailwayIdAsync(int stationId);
    Task<List<int>> GetNeighborStationTransitionIdAsync(int stationId);

    Task<(int, int)?> GetFromToStationIdByRailwayIdAsync(int railwayId);
    Task<(int, int)?> GetFromToStationIdByTransitionIdAsync(int transitionId);

    Task<int> UpdateChartByIdAsync(int chartId, MCMC.Chart chart);
    Task<int> UpdateBranchByIdAsync(int branchId, MCMC.Branch branch);
    Task<int> UpdateStationByIdAsync(int stationId, MCMC.Station station);
    Task<int> UpdateRailwayByIdAsync(int railwayId, MCMC.Railway railway);
    Task<int> UpdateTransitionByIdAsync(int stationId, MCMC.Transition transition);

    Task<int> DeleteChartByIdAsync(int id);

    Task<bool> IsCityTitleUniqueAsync(string city, string title);
    Task<bool> IsBranchTitleUniqueInChartAsync(string title, int chartId);
}