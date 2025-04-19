using MetroGid.Core.Models;

namespace MetroGid.DBA.Interfaces
{
    public interface IChartRepository
    {
        Task<int> AddAsync(Chart chart);

        Task<Chart> GetChartByIdAsync(int chartId);
        Task<Branch> GetBranchByIdAsync(int branchId);
        Task<Station> GetStationByIdAsync(int stationId);
        Task<Railway> GetRailwayByIdAsync(int railwayId);
        Task<Transition> GetTransitionByIdAsync(int transitionId);

        Task<int> GetChartIdAsync(string city, string title);
        Task<int> GetBranchIdAsync(string title, int chartId);
        Task<int> GetStationIdAsync(string title, int branchId);

        Task<List<int>> GetAllChartIdAsync();
        Task<List<int>> GetAllChartBranchIdAsync(int chartId);
        Task<List<int>> GetAllBranchStationIdAsync(int branchId);

        Task<List<ValueTuple<string, string>>> GetAllChartCityTitleAsync(); // <City, Title>
        Task<List<string>> GetAllChartBranchTitleAsync(int chartId);
        Task<List<string>> GetAllBranchStationTitleAsync(int branchId);

        Task<List<int>> GetNeighborStationRailwayAsync(int stationId);
        Task<List<int>> GetNeighborStationTransitionAsync(int stationId);

        Task<List<int>> GetFromToStationIdByRailwayIdAsync(int railwayId);
        Task<List<int>> GetFromToStationIdByTransitionIdAsync(int transitionId);

        Task UpdateChartByIdAsync(int chartId, Chart chart);
        Task UpdateBranchByIdAsync(int branchId, Branch branch);
        Task UpdateStationByIdAsync(int stationId, Station station);
        Task UpdateRailwayByIdAsync(int railwayId, Railway railway);
        Task UpdateTransitionByIdAsync(int stationId, Transition transition);

        Task DeleteChartByIdAsync(int id);

        Task<bool> IsCityTitleUniqueAsync(string city, string title);
        Task<bool> IsBranchTitleUniqueInChartAsync(string title, int chartId);
    }
}