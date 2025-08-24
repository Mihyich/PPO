using MetroGid.Controllers.DTO;

namespace MetroGid.Controllers.Interfaces;

public interface IRouteService
{
    Task<RouteDTO?> SearchRouteAsync(
        string city, string chartTitle,
        string branchSrcTitle, string stationSrcTitle,
        string branchDstTitle, string stationDstTitle,
        TimeOnly startTime);

    Task<int> SaveRouteAsync(RoleTypeDTO role, int clientId, RouteDTO route, int chartId);
    Task<List<string>> LookForSavedRoutesInChartAsync(RoleTypeDTO role, int clientId, int chartId); // просмотр сохраненных маршрутов в конкретной схеме метро, у конктретного пользователя
}