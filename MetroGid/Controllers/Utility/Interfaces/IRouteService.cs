using MetroGid.Controllers.Utility.DTO.Concrete;

namespace MetroGid.Controllers.Utility.Interfaces;

public interface IRouteService
{
    Task<RouteDTO?> SearchRouteAsync(
        string city, string chartTitle,
        string branchSrcTitle, string stationSrcTitle,
        string branchDstTitle, string stationDstTitle,
        TimeOnly startTime
    );

    Task<int> SaveRouteAsync(
        int clientId,
        int chartId,
        string routeJson
    );

    Task<List<string>> LookForSavedRoutesInChartAsync(
        int clientId,
        int chartId
    ); // просмотр сохраненных маршрутов в конкретной схеме метро, у конктретного пользователя
}