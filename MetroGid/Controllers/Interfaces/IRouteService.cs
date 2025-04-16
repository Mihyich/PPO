namespace MetroGid.Controllers.Interfaces
{
    public enum RouteServiceResult
    {
        INVALID_STATION_ID,    // некорректный айди станции
        UNEXPECTED_STATION_ID, // станции с разных веток
        SAME_STATION_ID,       // одинковые станции
        INVALID_ROLE           // нет прав
    }

    public interface IRouteService
    {
        Task<> SearchRoute(int src_id, int dst_id);
        TrajectoryServiceResult SaveRoute(int clientId, );
        List<Trajectory> LookForSavedTrajectoriesInChart(string login, string password, int id); // просмотр сохраненных маршрутов в конкретной схеме метро
    }
}