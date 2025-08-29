using MetroGid.Controllers.Utility.DTO.Concrete;

namespace MetroGid.Controllers.Utility.Interfaces;

public interface IChartService
{
    Task<int> AddChartAsync(string chartJson);

    Task<ChartDTO?> GetChartAsync(string cityTitle, string chartTitle);
    Task<string?> GetChartSchemeAsync(string cityTitle, string chartTitle);
    Task<List<ValueTuple<string, string>>> GetChartsCitiesTitlesAsync();

    Task<BranchDTO?> GetBranchAsync(string cityTitle, string chartTitle, string branchTitle);
    Task<List<string>> GetChartBranchTitlesAsync(int chartId);

    Task<StationDTO?> GetStationAsync(
        string cityTitle, string chartTitle,
        string branchTitle, string stationTitle);
    Task<List<string>> GetBranchStationTitlesAsync(string cityTitle, string chartTitle, string branchTitle);

    Task<TransitionDTO?> GetTransitionAsync(
        string cityTitle, string chartTitle,
        string fromBranchTitle, string fromStationTitle,
        string toBranchTitle, string toStationTitle);

    Task<RailwayDTO?> GetRailwayAsync(
        string cityTitle, string chartTitle,
        string branchTitle,
        string fromStationTitle, string toStationTitle);

    Task<int?> GetStationDutyIdAsync(
        string cityTitle, string chartTitle,
        string branchTitle, string stationTitle);

    Task<int?> GetTransitionDutyIdAsync(
        string cityTitle, string chartTitle,
        string fromBranchTitle, string fromStationTitle,
        string toBranchTitle, string toStationTitle);

    // Изменение атрибутов таблиц
    Task<int> UpdateChartAsync(
        RoleTypeDTO role,
        string chartCity, string chartTitle,
        ChartDTO chart);
    Task<int> UpdateChartSchemeAsync(
        RoleTypeDTO role,
        string chartCity, string chartTitle,
        string scheme
    );
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