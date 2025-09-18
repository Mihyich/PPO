using MCMC = MetroGid.Core.Models.Concrete;
using MCMA = MetroGid.Core.Models.Advanced;

namespace MetroGid.Controllers.Utility.Interfaces;

public interface IRouteService
{
    Task<MCMC.Route> SearchRouteAsync(
        string city, string chartTitle,
        string branchSrcTitle, string stationSrcTitle,
        string branchDstTitle, string stationDstTitle,
        TimeOnly startTime
    );

    Task<MCMA.IdRow> SaveRouteAsync(
        int clientId,
        MCMC.Route route
    );

    Task<MCMA.TitlesRow> GetSavedChartRoutesTitles(
        int clientId,
        string city,
        string chartTitle
    ); // просмотр сохраненных маршрутов в конкретной схеме метро, у конктретного пользователя (получить все названия)

    Task<MCMC.Route> GetSavedChart(
        int clientId,
        string city,
        string chartTitle,
        string title
    );

    Task<MCMA.DeletedRowCount> DeleteAsync(
        int clientId,
        string title
    );
}