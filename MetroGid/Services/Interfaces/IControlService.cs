using MetroGid.Services.Models;

namespace MetroGid.Services.Interfaces
{
    public enum ControlServiceResult
    {
        SUCCESS,
        INVALID_ROLE,
        INVALID_ID,
        INVALID_TITLE,
        INVALID_CITY,
        INVALID_SVG_INST,
        INVALID_COLOR,
        INVALID_ACCESS_TYPE,
        INVALID_CONVERT_TO_TIMEONLY,

    }

    public interface IControlService
    {
        // Изменение атрибутов таблицы Chart
        ControlServiceResult UpdateChartTitle(ClientRole role, int id, string title);
        ControlServiceResult UpdateChartCity(ClientRole role, int id, string city);
        ControlServiceResult UpdateChartSvg_inst(ClientRole role, int id, string svg_inst);

        // Изменение атрибутов таблицы Branch
        ControlServiceResult UpdateBranchTitle(ClientRole role, int id, string title);
        ControlServiceResult UpdateBranchColor(ClientRole role, int id, int color);
        ControlServiceResult UpdateBranchAccessType(ClientRole role, int id, AccessType access_type);

        // Изменение атрибутов таблицы Station
        ControlServiceResult UpdateStationTitle(ClientRole role, int id, string title);
        ControlServiceResult UpdateStationOccupancy(ClientRole role, int id, int occupancy);
        ControlServiceResult UpdateStationAccessType(ClientRole role, int id, AccessType access_type);
        ControlServiceResult UpdateStationOpenTime(ClientRole role, int id, string time); // Время ожидается конвертируемым в TimeOnly
        ControlServiceResult UpdateStationCloseTime(ClientRole role, int id, string time); // Время ожидается конвертируемым в TimeOnly

        // Изменение атрибутов таблицы Transition
        ControlServiceResult UpdateTransitionOccupancy(ClientRole role, int id, int occupancy);
        ControlServiceResult UpdateTransitionAccessType(ClientRole role, int id, AccessType access_type);
        ControlServiceResult UpdateTransitionOpenTime(ClientRole role, int id, string time); // Время ожидается конвертируемым в TimeOnly
        ControlServiceResult UpdateTransitionCloseTime(ClientRole role, int id, string time); // Время ожидается конвертируемым в TimeOnly

        // Изменение атрибутов таблицы StationStation
        ControlServiceResult UpdateStationStationDuration(ClientRole role, int id, string duration); // Время ожидается конвертируемым в TimeOnly
    }
}