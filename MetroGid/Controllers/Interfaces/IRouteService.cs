using MetroGid.Controllers.DTO;

namespace MetroGid.Controllers.Interfaces
{
    public interface IRouteService
    {
        Task<RouteDTO> SearchRoute(
            string city, string chartTitle,
            string branchSrcTitle, string stationSrcTitle,
            string branchDstTitle, string stationDstTitle);

        Task SaveRoute(int clientId, RouteDTO route, int chartId);
        Task<List<RouteDTO>> LookForSavedRoutesInChart(int clientId, int chartId); // просмотр сохраненных маршрутов в конкретной схеме метро, у конктретного пользователя
    }
}