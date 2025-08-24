using MetroGid.Controllers.DTO;

namespace MetroGid.Controllers.Interfaces;

public interface IChartService
{
    Task<ChartDTO?> GetChartAsync(string cityTitle, string chartTitle);
    Task<List<ValueTuple<string, string>>> GetChartsCitiesTitlesAsync();

    Task<BranchDTO?> GetBranchAsync(string cityTitle, string chartTitle, string branchTitle);
    Task<List<string>> GetChartBranchTitlesAsync(int chartId);

    Task<StationDTO?> GetStationAsync(
        string cityTitle, string chartTitle,
        string branchTitle, string stationTitle);
    Task<List<string>> GetBranchStationTitlesAsync(string cityTitle, string chartTitle, string branchTitle);

    // Изменение атрибутов таблиц
    Task<int> UpdateChartAsync(
        RoleTypeDTO role,
        string chartCity, string chartTitle,
        ChartDTO chart);
    Task<int> UpdateBranchAsync(
        RoleTypeDTO role,
        string chartCity, string chartTitle,
        string branchTitle,
        BranchDTO branch);
    Task<int> UpdateStationAsync(
        RoleTypeDTO role,
        string chartCity, string chartTitle,
        string branchTitle, string stationTitle,
        StationDTO station);
    Task<int> UpdateRailwayAsync(
        RoleTypeDTO role,
        string chartCity, string chartTitle,
        string BranchTitle,
        string fromStationTitle, string toStationTitle,
        RailwayDTO railway);
    Task<int> UpdateTransitionAsync(
        RoleTypeDTO role,
        string chartCity, string chartTitle,
        string fromBranchTitle, string fromStationTitle,
        string toBranchTitle, string toStationTitle,
        TransitionDTO transition);
}