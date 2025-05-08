using MetroGid.Controllers.DTO;

namespace MetroGid.Controllers.Interfaces;

public interface IChartService
{
    Task<ChartDTO> GetChart(int chartId);
    Task<int> GetChartId(string city, string title);
    Task<List<ValueTuple<string, string>>> GetChartsCitiesTitles();

    Task<BranchDTO> GetBranch(int branchId);
    Task<int> GetChartBranchId(string title, int chartId);
    Task<List<string>> GetChartBranchTitles(int chartId);

    Task<StationDTO> GetStation(int stationId);
    Task<int> GetBranchStationId(string title, int branchId);
    Task<List<string>> GetBranchStationTitles(int branchId);

    // Изменение атрибутов таблиц
    Task UpdateChart(int chartId, ChartDTO chart);
    Task UpdateBranch(int branchId, BranchDTO branch);
    Task UpdateStation(int stationId, StationDTO station);
    Task UpdateTransition(int transitionId, TransitionDTO transition);
}