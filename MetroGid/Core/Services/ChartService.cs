using MetroGid.Controllers.DTO;
using MetroGid.Controllers.Interfaces;
using MetroGid.Core.Models.Concrete;
using MetroGid.Core.Utilities;
using MetroGid.DBA.Interfaces;

namespace MetroGid.Core.Services
{
    public class ChartService(IChartRepository chartRepo) : IChartService
    {
        private readonly IChartRepository ChartRepo = chartRepo;

        public async Task<ChartDTO> GetChart(int chartId) =>
            CntChart.Convert(await ChartRepo.GetChartWeakByIdAsync(chartId));

        public async Task<int> GetChartId(string city, string title) =>
            await ChartRepo.GetChartIdAsync(city, title);

        public async Task<List<ValueTuple<string, string>>> GetChartsCitiesTitles() =>
            await ChartRepo.GetAllChartCityTitleAsync();

        public async Task<BranchDTO> GetBranch(int branchId) =>
            CntBranch.Convert(await ChartRepo.GetBranchWeakByIdAsync(branchId));

        public async Task<int> GetChartBranchId(string title, int chartId) =>
            await ChartRepo.GetBranchIdAsync(title, chartId);

        public async Task<List<string>> GetChartBranchTitles(int chartId) =>
            await ChartRepo.GetAllChartBranchTitleAsync(chartId);

        public async Task<StationDTO> GetStation(int stationId) =>
            CntStation.Convert(await ChartRepo.GetStationWeakByIdAsync(stationId));

        public async Task<int> GetBranchStationId(string title, int branchId) =>
            await ChartRepo.GetStationIdAsync(title, branchId);

        public async Task<List<string>> GetBranchStationTitles(int branchId) =>
            await ChartRepo.GetAllBranchStationTitleAsync(branchId);


        // Изменение атрибутов таблицы Chart
        public async Task UpdateChartTitle(string title, int chartId)
        {
            Chart chart = await ChartRepo.GetChartWeakByIdAsync(chartId);
            chart.Title = title;
            await ChartRepo.UpdateChartByIdAsync(chartId, chart);
        }

        public async Task UpdateChartCity(string city, int chartId)
        {
            Chart chart = await ChartRepo.GetChartWeakByIdAsync(chartId);
            chart.City = city;
            await ChartRepo.UpdateChartByIdAsync(chartId, chart);
        }

        public async Task UpdateChartSvg_inst(string svgInst, int chartId)
        {
            Chart chart = await ChartRepo.GetChartWeakByIdAsync(chartId);
            chart.SvgInst = svgInst;
            await ChartRepo.UpdateChartByIdAsync(chartId, chart);
        }


        // Изменение атрибутов таблицы Branch
        public async Task UpdateBranchTitle(string title, int branchId)
        {
            Branch branch = await ChartRepo.GetBranchWeakByIdAsync(branchId);
            branch.Title = title;
            await ChartRepo.UpdateBranchByIdAsync(branchId, branch);
        }

        public async Task UpdateBranchColor(int color, int branchId)
        {
            Branch branch = await ChartRepo.GetBranchWeakByIdAsync(branchId);
            branch.Color = color;
            await ChartRepo.UpdateBranchByIdAsync(branchId, branch);
        }

        public async Task UpdateBranchAccessType(AccessTypeDTO type, int branchId)
        {
            Branch branch = await ChartRepo.GetBranchWeakByIdAsync(branchId);
            branch.Type = CntAccessType.Convert(type);
            await ChartRepo.UpdateBranchByIdAsync(branchId, branch);
        }


        // Изменение атрибутов таблицы Station
        public async Task UpdateStationTitle(string title, int stationId)
        {
            Station station = await ChartRepo.GetStationWeakByIdAsync(stationId);
            station.Title = title;
            await ChartRepo.UpdateStationByIdAsync(stationId, station);
        }

        public async Task UpdateStationOccupancy(int occupancy, int stationId)
        {
            Station station = await ChartRepo.GetStationWeakByIdAsync(stationId);
            station.Occupancy = occupancy;
            await ChartRepo.UpdateStationByIdAsync(stationId, station);
        }

        public async Task UpdateStationAccessType(AccessTypeDTO type, int stationId)
        {
            Station station = await ChartRepo.GetStationWeakByIdAsync(stationId);
            station.Type = CntAccessType.Convert(type);
            await ChartRepo.UpdateStationByIdAsync(stationId, station);
        }

        public async Task UpdateStationOpenTime(string time, int stationId)
        {
            Station station = await ChartRepo.GetStationWeakByIdAsync(stationId);
            station.OpenTime = TimeConverter.FromString(time);
            await ChartRepo.UpdateStationByIdAsync(stationId, station);
        }

        public async Task UpdateStationCloseTime(string time, int stationId)
        {
            Station station = await ChartRepo.GetStationWeakByIdAsync(stationId);
            station.CloseTime = TimeConverter.FromString(time);
            await ChartRepo.UpdateStationByIdAsync(stationId, station);
        }


        // Изменение атрибутов таблицы Transition
        public async Task UpdateTransitionOccupancy(int occupancy, int transitionId)
        {
            Transition transition = await ChartRepo.GetTransitionByIdAsync(transitionId);
            transition.Occupancy = occupancy;
            await ChartRepo.UpdateTransitionByIdAsync(transitionId, transition);
        }

        public async Task UpdateTransitionAccessType(AccessTypeDTO type, int transitionId)
        {
            Transition transition = await ChartRepo.GetTransitionByIdAsync(transitionId);
            transition.Type = CntAccessType.Convert(type);
            await ChartRepo.UpdateTransitionByIdAsync(transitionId, transition);
        }

        public async Task UpdateTransitionDuration(string time, int transitionId)
        {
            Transition transition = await ChartRepo.GetTransitionByIdAsync(transitionId);
            transition.Duration = TimeConverter.FromString(time);
            await ChartRepo.UpdateTransitionByIdAsync(transitionId, transition);
        }

        public async Task UpdateTransitionOpenTime(string time, int transitionId)
        {
            Transition transition = await ChartRepo.GetTransitionByIdAsync(transitionId);
            transition.OpenTime = TimeConverter.FromString(time);
            await ChartRepo.UpdateTransitionByIdAsync(transitionId, transition);
        }

        public async Task UpdateTransitionCloseTime(string time, int transitionId)
        {
            Transition transition = await ChartRepo.GetTransitionByIdAsync(transitionId);
            transition.CloseTime = TimeConverter.FromString(time);
            await ChartRepo.UpdateTransitionByIdAsync(transitionId, transition);
        }
    }
}