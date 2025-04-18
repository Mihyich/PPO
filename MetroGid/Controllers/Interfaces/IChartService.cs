using MetroGid.Controllers.DTO;

namespace MetroGid.Controllers.Interfaces
{
    public interface IChartService
    {
        Task<ChartDTO> GetChart(int chartId);
        Task<int> GetChartId(string city, string title);
        Task<List<ValueTuple<string, string>>> GetChartsCitiesTitles();

        Task<BranchDTO> GetBranch(int branchId);
        Task<int> GetBranchId(string title, int chartId);
        Task<List<string>> GetBranchTitles(int chartId);

        Task<StationDTO> GetStation(int stationId);
        Task<int> GetStationId(string title, int branchId);
        Task<List<string>> GetStationTitles(int branchId);

        Task<List<string>> GetNeighnorStationTitles(int stationId);

        Task<int> GetRailwayId(int stationSrcId, int stationDstId);
        // Task<>

        // Изменение атрибутов таблицы Chart
        Task UpdateChartTitle(string title, int chartId);
        Task UpdateChartCity(string city, int chartId);
        Task UpdateChartSvg_inst(string svgInst, int chartId);

        // Изменение атрибутов таблицы Branch
        Task UpdateBranchTitle(string title, int branchId);
        Task UpdateBranchColor(int color, int branchId);
        Task UpdateBranchAccessType(AccessTypeDTO access_type, int branchId);

        // Изменение атрибутов таблицы Station
        Task UpdateStationTitle(string title, int stationId);
        Task UpdateStationOccupancy(int occupancy, int stationId);
        Task UpdateStationAccessType(AccessTypeDTO access_type, int stationId);
        Task UpdateStationOpenTime(string time, int stationId); // Время ожидается конвертируемым в TimeOnly
        Task UpdateStationCloseTime(string time, int stationId); // Время ожидается конвертируемым в TimeOnly

        // Изменение атрибутов таблицы Transition
        Task UpdateTransitionOccupancy(int occupancy, int transitionId);
        Task UpdateTransitionAccessType(AccessTypeDTO access_type, int transitionId);
        Task UpdateTransitionDuration(string time, int transitionId);
        Task UpdateTransitionOpenTime(string time, int transitionId); // Время ожидается конвертируемым в TimeOnly
        Task UpdateTransitionCloseTime(string time, int transitionId); // Время ожидается конвертируемым в TimeOnly
    }
}