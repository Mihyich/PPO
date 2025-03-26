using MetroGid.Services.Models;

namespace MetroGid.Services.Interfaces
{
    public enum TrajectoryServiceResult
    {
        SUCCESS = 0,
        INVALID_STATION_ID,    // некорректный айди станции
        UNEXPECTED_STATION_ID, // станции с разных веток
        SAME_STATION_ID,       // одинковые станции
        INVALID_ROLE           // нет прав
    }

    public interface ITrajectoryService
    {
        Trajectory SearchTrajectory(int src_id, int dst_id, out TrajectoryServiceResult error);
        TrajectoryServiceResult SaveTrajectory(string login, string password, Trajectory trajectory);
        List<Trajectory> LookForSavedTrajectoriesInChart(string login, string password, int id); // просмотр сохраненных маршрутов в конкретной схеме метро
    }
}