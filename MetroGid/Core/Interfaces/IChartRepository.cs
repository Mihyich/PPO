using MetroGid.Core.Models.Concrete;

namespace MetroGid.Core.Interfaces;

public interface IChartRepository
{
    Task<int> AddAsync(Chart chart);

    Task<string> GetChartJsonByIdAsync(int chartId);
    Task<string> GetChartJsonAsync(string city, string title);
    
    Task<Chart> GetChartWeakByIdAsync(int chartId);
    Task<Branch> GetBranchWeakByIdAsync(int branchId);
    Task<Station> GetStationWeakByIdAsync(int stationId);
    Task<Railway> GetRailwayByIdAsync(int railwayId);
    Task<Transition> GetTransitionByIdAsync(int transitionId);

    Task<int> GetChartIdAsync(string city, string title);
    Task<int> GetBranchIdAsync(string title, int chartId);
    Task<int> GetStationIdAsync(string title, int branchId);

    Task<List<int>> GetAllChartIdAsync();
    Task<List<int>> GetAllChartBranchIdAsync(int chartId);
    Task<List<int>> GetAllBranchStationIdAsync(int branchId);

    Task<(string, string)> GetAllChartCityTitleAsync(); // <City, Title>
    Task<List<string>> GetAllChartBranchTitleAsync(int chartId);
    Task<List<string>> GetAllBranchStationTitleAsync(int branchId);

    Task<(int, int)> GetNeighborStationRailwayIdAsync(int stationId);
    Task<List<int>> GetNeighborStationTransitionIdAsync(int stationId);

    Task<(int, int)> GetFromToStationIdByRailwayIdAsync(int railwayId);
    Task<(int, int)> GetFromToStationIdByTransitionIdAsync(int transitionId);

    Task UpdateChartByIdAsync(int chartId, Chart chart);
    Task UpdateBranchByIdAsync(int branchId, Branch branch);
    Task UpdateStationByIdAsync(int stationId, Station station);
    Task UpdateRailwayByIdAsync(int railwayId, Railway railway);
    Task UpdateTransitionByIdAsync(int transitionId, Transition transition);

    Task DeleteChartByIdAsync(int id);
}