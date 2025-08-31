using ConsoleClient.SharedDTO.Auth;
using ConsoleClient.SharedDTO.Concrete;
using MetroGid.Controllers.Utility.DTO.Concrete;

namespace ConsoleClient.Services.Interfaces;

public interface IApiService
{
    Task<AuthResponseDTO?> RegAsync(
        string login,
        string password,
        string mail
    );

    Task<AuthResponseDTO?> LogInAsync(
        string login,
        string password
    );

    Task LogOutAsync();

    Task UnRegAsync(
        string password
    );

    Task AddChart(
        string ChartJson
    );

    Task PatchChartSceme(
        string CityTitle,
        string ChartTitle,
        string Scheme
    );

    Task PatchBranch(
        string CityTitle,
        string ChartTitle,
        string BranchTitle,

        string NewTitle,
        string NewColor, // hex формат
        string NewAccess
    );

    Task PatchStation(
        string CityTitle,
        string ChartTitle,
        string BranchTitle,
        string StationTitle,

        string NewTitle,
        int NewOccupancy,
        string NewAccess,
        string NewOpenTime,
        string NewCloseTime
    );

    Task PatchTransition(
        string CityTitle,
        string ChartTitle,
        string FromBranchTitle,
        string FromStationTitle,
        string ToBranchTitle,
        string ToStationTitle,

        int NewOccupancy,
        string NewAccess,
        string NewDuration,
        string NewOpenTime,
        string NewCloseTime
    );

    Task PatchRailway(
        string CityTitle,
        string ChartTitle,

        string BranchTitle,
        string DutyStationTitle,
        string ToStationTitle,

        string NewDuration
    );

    Task<List<ChartDTO>> GetChartsCitiesTitles();

    Task<string?> GetScheme(
        string CityTitle,
        string ChartTitle
    );

    Task<RouteDTO?> SearchRoute(
        string CityTitle,
        string ChartTitle,
        string FromBranchTitle,
        string FromStationTitle,
        string ToBranchTitle,
        string ToStationTitle,
        string curTime
    );

    Task<int> SaveRoute(
        string routeJson
    );

    Task<List<string>> GetRouteTitles(
        string CityTitle,
        string ChartTitle
    );

    Task<RouteDTO?> GetSavedRoute(
        string CityTitle,
        string ChartTitle,
        string RouteTitle
    );

    Task<int> DeleteRoute(
        string CityTitle,
        string ChartTitle,
        string RouteTitle
    );
}