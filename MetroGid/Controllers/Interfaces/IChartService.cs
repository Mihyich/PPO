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
    Task<int> UpdateChart(
        RoleTypeDTO role,
        string chartCity, string chartTitle,
        ChartDTO chart);
    Task<int> UpdateBranch(
        RoleTypeDTO role,
        string chartCity, string chartTitle,
        string branchTitle,
        BranchDTO branch);
    Task<int> UpdateStation(
        RoleTypeDTO role,
        string chartCity, string chartTitle,
        string branchTitle, string stationTitle,
        StationDTO station);
    Task<int> UpdateRailway(
        RoleTypeDTO role,
        string chartCity, string chartTitle,
        string BranchTitle,
        string fromStationTitle, string toStationTitle,
        RailwayDTO railway);
    Task<int> UpdateTransition(
        RoleTypeDTO role,
        string chartCity, string chartTitle,
        string fromBranchTitle, string fromStationTitle,
        string toBranchTitle, string toStationTitle,
        TransitionDTO transition);
}