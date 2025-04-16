namespace MetroGid.Controllers.Interfaces
{
    public enum ControlServiceResult
    {
        SUCCESS = 0,
        INVALID_ROLE,
        INVALID_ID,
        INVALID_TITLE,
        INVALID_CITY,
        INVALID_SVG_INST,
        INVALID_COLOR,
        INVALID_ACCESS_TYPE,
        INVALID_CONVERT_TO_TIMEONLY
    }

    public interface IControlService
    {
        // Изменение атрибутов таблицы Chart
        ControlServiceResult UpdateChartTitle(string login, string password, int id, string title);
        ControlServiceResult UpdateChartCity(string login, string password, int id, string city);
        ControlServiceResult UpdateChartSvg_inst(string login, string password, int id, string svg_inst);

        // Изменение атрибутов таблицы Branch
        ControlServiceResult UpdateBranchTitle(string login, string password, int id, string title);
        ControlServiceResult UpdateBranchColor(string login, string password, int id, int color);
        ControlServiceResult UpdateBranchAccessType(string login, string password, int id, AccessType access_type);

        // Изменение атрибутов таблицы Station
        ControlServiceResult UpdateStationTitle(string login, string password, int id, string title);
        ControlServiceResult UpdateStationOccupancy(string login, string password, int id, int occupancy);
        ControlServiceResult UpdateStationAccessType(string login, string password, int id, AccessType access_type);
        ControlServiceResult UpdateStationOpenTime(string login, string password, int id, string time); // Время ожидается конвертируемым в TimeOnly
        ControlServiceResult UpdateStationCloseTime(string login, string password, int id, string time); // Время ожидается конвертируемым в TimeOnly

        // Изменение атрибутов таблицы Transition
        ControlServiceResult UpdateTransitionOccupancy(string login, string password, int id, int occupancy);
        ControlServiceResult UpdateTransitionAccessType(string login, string password, int id, AccessType access_type);
        ControlServiceResult UpdateTransitionOpenTime(string login, string password, int id, string time); // Время ожидается конвертируемым в TimeOnly
        ControlServiceResult UpdateTransitionCloseTime(string login, string password, int id, string time); // Время ожидается конвертируемым в TimeOnly

        // Изменение атрибутов таблицы StationStation
        ControlServiceResult UpdateStationStationDuration(string login, string password, int id, string duration); // Время ожидается конвертируемым в TimeOnly
    }
}