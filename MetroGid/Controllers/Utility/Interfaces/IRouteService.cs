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

    Task<int> SaveRouteAsync(ClientDTO client, RouteDTO route, int chartId);
    Task<List<string>> LookForSavedRoutesInChartAsync(ClientDTO client, int chartId); // просмотр сохраненных маршрутов в конкретной схеме метро, у конктретного пользователя
}