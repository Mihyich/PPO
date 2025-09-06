using MetroGid.Controllers.Utility.DTO.Concrete;
using MCMC = MetroGid.Core.Models.Concrete;
using MCMT = MetroGid.Core.Models.Types;

namespace MetroGid.Controllers.Utility.Interfaces;

public interface IChartService
{
    Task<int> AddChartAsync(string chartJson);

    Task<int> GetChartIdAsync(string cityTitle, string chartTitle);
    Task<ChartDTO?> GetChartAsync(string cityTitle, string chartTitle);
    Task<string?> GetChartSchemeAsync(string cityTitle, string chartTitle);
    Task<List<ValueTuple<string, string>>> GetChartsCitiesTitlesAsync();

    Task<BranchDTO?> GetBranchAsync(
        string cityTitle, string chartTitle,
        string branchTitle
    );
    Task<List<string>> GetChartBranchTitlesAsync(int chartId);

    Task<StationDTO?> GetStationAsync(
        string cityTitle, string chartTitle,
        string branchTitle, string stationTitle
    );
    Task<List<string>> GetBranchStationTitlesAsync(
        string cityTitle, string chartTitle,
        string branchTitle
    );

    Task<TransitionDTO?> GetTransitionAsync(
        string cityTitle, string chartTitle,
        string fromBranchTitle, string fromStationTitle,
        string toBranchTitle, string toStationTitle
    );

    Task<RailwayDTO?> GetRailwayAsync(
        string cityTitle, string chartTitle,
        string branchTitle,
        string fromStationTitle, string toStationTitle
    );

    Task<int?> GetStationDutyIdAsync(
        string cityTitle, string chartTitle,
        string branchTitle, string stationTitle
    );

    Task<int?> GetTransitionDutyIdAsync(
        string cityTitle, string chartTitle,
        string fromBranchTitle, string fromStationTitle,
        string toBranchTitle, string toStationTitle
    );

    // Изменение атрибутов таблиц
    Task<int> UpdateChartSchemeAsync(
        MCMT.RoleType role,
        string chartCity, string chartTitle,
        string scheme
    );
    Task<int> UpdateBranchAsync(
        MCMT.RoleType role,
        string chartCity, string chartTitle,
        string branchTitle,
        MCMC.Branch branch
    );
    Task<int> UpdateStationAsync(
        MCMT.RoleType role,
        string chartCity, string chartTitle,
        string branchTitle, string stationTitle,
        MCMC.Station station
    );
    Task<int> UpdateRailwayAsync(
        MCMT.RoleType role,
        string chartCity, string chartTitle,
        string BranchTitle,
        string fromStationTitle, string toStationTitle,
        MCMC.Railway railway
    );
    Task<int> UpdateTransitionAsync(
        MCMT.RoleType role,
        string chartCity, string chartTitle,
        string fromBranchTitle, string fromStationTitle,
        string toBranchTitle, string toStationTitle,
        MCMC.Transition transition
    );
}