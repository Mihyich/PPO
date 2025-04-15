using MetroGid.Services.Models;

namespace MetroGid.DBA.Interfaces
{
    public interface IChartRepository
    {
        Task<int> AddAsync(Chart chart);

        Task<int> GetChartIdAsync(string city, string title);
        Task<List<ValueTuple<string, string>>?> GetAllChartCitiesTitlesAsync(); // <City, Title>

        Task<int> GetBranchIdAsync(string title, int chartId);
        Task<List<string>?> GetAllChartBranchTitlesAsync(int chartId);

        Task<int> GetStationIdAsync(string title, int chartId);
        Task<List<string>?> GetAllBranchStationsTitlesAsync(int branchId);

        Task<List<int>?> GetAllStationRailwaysAsync(int stationId);
        Task<List<int>?> GetAllStationTransitionsAsync(int stationId);

        Task<ValueTuple<string, string>?> GetFromToTitlesByRailwayIdAsync(int railwayId);
        Task<ValueTuple<string, string, string, string>?> GetFromToTitlesByTransitionIdAsync(int transitionId); // BranchTitleSrc StationTitleSrc -> BranchTitleDst StationTitleDst

        Task<Chart?> GetChartByIdAsync(int chartId);
        Task<Branch?> GetBranchByIdAsync(int branchId);
        Task<Station?> GetStationByIdAsync(int stationId);
        Task<Railway?> GetRailwayByIdAsync(int railwayId);
        Task<Transition?> GetTransitionByIdAsync(int transitionId);

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