using MetroGid.Core.Models.Concrete;

namespace MetroGid.Core.Interfaces;

public interface IChartRepository
{
    Task<int> AddAsync(Chart chart);

    Task<int> GetChartIdAsync(string city, string title);
    Task<int> GetBranchIdAsync(string title, int chartId);
    Task<int> GetStationIdAsync(string title, int branchId);

    Task<List<int>> GetAllChartIdAsync();
    Task<List<int>> GetAllChartBranchIdAsync(int chartId);
    Task<List<int>> GetAllBranchStationIdAsync(int branchId);

    Task<string?> GetChartJsonByIdAsync(int chartId);
    Task<Chart?> GetChartWeakByIdAsync(int chartId);
    Task<Branch?> GetBranchWeakByIdAsync(int branchId);
    Task<Station?> GetStationWeakByIdAsync(int stationId);
    Task<Railway?> GetRailwayByIdAsync(int railwayId);
    Task<Transition?> GetTransitionByIdAsync(int transitionId);

    Task<string?> GetChartJsonByCredentialsAsync(string city, string title);

    Task<(string, string)> GetAllChartCityTitleAsync(); // <City, Title>
    Task<List<string>> GetAllChartBranchTitleAsync(int chartId);
    Task<List<string>> GetAllBranchStationTitleAsync(int branchId);

    Task<(int, int)> GetNeighborStationRailwayIdAsync(int stationId);
    Task<List<int>> GetNeighborStationTransitionIdAsync(int stationId);

    Task<(int, int)> GetFromToStationIdByRailwayIdAsync(int railwayId);
    Task<(int, int)> GetFromToStationIdByTransitionIdAsync(int transitionId);

    Task<int> UpdateChartByIdAsync(int chartId, Chart chart);
    Task<int> UpdateBranchByIdAsync(int branchId, Branch branch);
    Task<int> UpdateStationByIdAsync(int stationId, Station station);
    Task<int> UpdateRailwayByIdAsync(int railwayId, Railway railway);
    Task<int> UpdateTransitionByIdAsync(int stationId, Transition transition);

    Task<int> DeleteChartByIdAsync(int id);

    Task<bool> IsCityTitleUniqueAsync(string city, string title);
    Task<bool> IsBranchTitleUniqueInChartAsync(string title, int chartId);
}