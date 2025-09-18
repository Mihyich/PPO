namespace MetroGid.Core.Exceptions.Truistic;

public class SavedRouteNotFoundException : Exception
{
    public int CliendId { get; }
    public int ChartId{ get; }
    public string RouteTitle { get; }

    public SavedRouteNotFoundException(
        int clientId, int chartId, string routeTitle
    ) : base($"Сохраненный маршрут '{routeTitle}' схема с айди {chartId} для пользователя с айди {clientId} не найден")
    {
        CliendId = clientId;
        ChartId = chartId;
        RouteTitle = routeTitle;
    }
}