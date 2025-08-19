using MetroGid.Controllers.DTO;

namespace MetroGid.Controllers.Interfaces;

public interface IChartService
{
    Task<ChartDTO?> GetChart(int chartId);
    Task<int> GetChartId(string city, string title);
    Task<List<ValueTuple<string, string>>> GetChartsCitiesTitles();

    Task<BranchDTO?> GetBranch(int branchId);
    Task<int> GetChartBranchId(string title, int chartId);
    Task<List<string>> GetChartBranchTitles(int chartId);

    Task<StationDTO?> GetStation(int stationId);
    Task<int> GetBranchStationId(string title, int branchId);
    Task<List<string>> GetBranchStationTitles(int branchId);

    // Изменение атрибутов таблиц
    Task<int> UpdateChart(RoleTypeDTO role, int chartId, ChartDTO chart);
    Task<int> UpdateBranch(RoleTypeDTO role, int branchId, BranchDTO branch);
    Task<int> UpdateStation(RoleTypeDTO role, int stationId, StationDTO station);
    Task<int> UpdateTransition(RoleTypeDTO role, int transitionId, TransitionDTO transition);
}