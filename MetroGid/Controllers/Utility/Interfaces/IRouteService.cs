using MetroGid.Controllers.Utility.DTO.Concrete;
using MCMC = MetroGid.Core.Models.Concrete;

namespace MetroGid.Controllers.Utility.Interfaces;

public interface IRouteService
{
    Task<MCMC.Route?> SearchRouteAsync(
        string city, string chartTitle,
        string branchSrcTitle, string stationSrcTitle,
        string branchDstTitle, string stationDstTitle,
        TimeOnly startTime
    );

    Task<int> SaveRouteAsync(
        int clientId,
        MCMC.Route route
    );

    Task<List<string>> GetSavedChartRoutesTitles(
        int clientId,
        string city,
        string chartTitle
    ); // просмотр сохраненных маршрутов в конкретной схеме метро, у конктретного пользователя (получить все названия)

    Task<MCMC.Route?> GetSavedChart(
        int clientId,
        string city,
        string chartTitle,
        string title
    );

    Task<int> DeleteAsync(
        int clientId,
        string title
    );
}