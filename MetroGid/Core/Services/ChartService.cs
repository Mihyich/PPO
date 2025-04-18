using MetroGid.Controllers.DTO;
using MetroGid.Controllers.Interfaces;
using MetroGid.Core.Exceptions;
using MetroGid.Core.Models;
using MetroGid.Core.Utilities;
using MetroGid.DBA.Interfaces;

namespace MetroGid.Core.Services
{
    public class ChartService(IChartRepository chartRepo) : IChartService
    {
        private readonly IChartRepository ChartRepo = chartRepo;

        public async Task<ChartDTO> GetChart(int chartId)
        {
            Chart chart = await ChartRepo.GetChartByIdAsync(chartId) ??
                throw new NotFoundException(
                    NotFoundException.ErrorType.ChartIdMisMatch,
                    $"Не найдена схема с айди {chartId}"
                );

            return CntChart.Convert(chart);
        }

        public async Task<int> GetChartId(string city, string title)
        {
            int id = await ChartRepo.GetChartIdAsync(city, title);

            if (id < 0)
                throw new NotFoundException(
                    NotFoundException.ErrorType.ChartCityTitleMisMatch,
                    $"Не найдена схема в городе '{city}' с названием '{title}'"
                );

            return id;
        }

        public async Task<List<ValueTuple<string, string>>> GetChartsCitiesTitles() =>
            await ChartRepo.GetAllChartCityTitleAsync() ?? [];


        public async Task<BranchDTO> GetBranch(int branchId)
        {
            Branch branch = await ChartRepo.GetBranchByIdAsync(branchId) ??
                throw new NotFoundException(
                    NotFoundException.ErrorType.BranchIdMisMatch,
                    $"Не найдена ветка с йади {branchId}"
                );

            return CntBranch.Convert(branch);
        }

        public async Task<int> GetChartBranchId(string title, int chartId)
        {
            int id = await ChartRepo.GetBranchIdAsync(title, chartId);

            if (id < 0)
                throw new NotFoundException(
                    NotFoundException.ErrorType.BranchTitleChartIdMisMatch,
                    $"Не найдена ветка '{title}' в схема с айди {chartId}"
                );

            return id;
        }

        public async Task<List<string>> GetChartBranchTitles(int chartId) =>
            await ChartRepo.GetAllChartBranchTitleAsync(chartId) ?? [];

        
        public async Task<StationDTO> GetStation(int stationId)
        {
            Station station = await ChartRepo.GetStationByIdAsync(stationId) ?? 
                throw new NotFoundException(
                    NotFoundException.ErrorType.StationIdMisMatch,
                    $"Не найдена станция с айди {stationId}"
                );

            return CntStation.Convert(station);
        }

        public async Task<int> GetBranchStationId(string title, int branchId)
        {
            int id = await ChartRepo.GetStationIdAsync(title, branchId);

            if (id < 0)
                throw new NotFoundException(
                    NotFoundException.ErrorType.StationTitleBranchIdMisMatch,
                    $"Не найдена станция '{title}' на ветка с айди {branchId}"
                );

            return id;
        }

        public async Task<List<string>> GetBranchStationTitles(int branchId) =>
            await ChartRepo.GetAllBranchStationTitleAsync(branchId) ?? [];


        // Изменение атрибутов таблицы Chart
        public async Task UpdateChartTitle(string title, int chartId)
        {
            Chart chart = await ChartRepo.GetChartByIdAsync(chartId) ??
                throw new NotFoundException(
                    NotFoundException.ErrorType.ChartIdMisMatch,
                    $"Не найдена схема с айди {chartId}"
                );

            chart.Title = title;

            await ChartRepo.UpdateChartByIdAsync(chartId, chart);
        }

        public async Task UpdateChartCity(string city, int chartId)
        {
            Chart chart = await ChartRepo.GetChartByIdAsync(chartId) ??
                throw new NotFoundException(
                    NotFoundException.ErrorType.ChartIdMisMatch,
                    $"Не найдена схема с айди {chartId}"
                );

            chart.City = city;

            await ChartRepo.UpdateChartByIdAsync(chartId, chart);
        }

        public async Task UpdateChartSvg_inst(string svgInst, int chartId)
        {
            Chart chart = await ChartRepo.GetChartByIdAsync(chartId) ??
                throw new NotFoundException(
                    NotFoundException.ErrorType.ChartIdMisMatch,
                    $"Не найдена схема с айди {chartId}"
                );

            chart.SvgInst = svgInst;

            await ChartRepo.UpdateChartByIdAsync(chartId, chart);
        }


        // Изменение атрибутов таблицы Branch
        public async Task UpdateBranchTitle(string title, int branchId)
        {
            Branch branch = await ChartRepo.GetBranchByIdAsync(branchId) ??
                throw new NotFoundException(
                    NotFoundException.ErrorType.BranchIdMisMatch,
                    $"Не найдена ветка с айди {branchId}"
                );

            branch.Title = title;

            await ChartRepo.UpdateBranchByIdAsync(branchId, branch);
        }

        public async Task UpdateBranchColor(int color, int branchId)
        {
            Branch branch = await ChartRepo.GetBranchByIdAsync(branchId) ??
                throw new NotFoundException(
                    NotFoundException.ErrorType.BranchIdMisMatch,
                    $"Не найдена ветка с айди {branchId}"
                );

            branch.Color = color;

            await ChartRepo.UpdateBranchByIdAsync(branchId, branch);
        }

        public async Task UpdateBranchAccessType(AccessTypeDTO type, int branchId)
        {
            Branch branch = await ChartRepo.GetBranchByIdAsync(branchId) ??
                throw new NotFoundException(
                    NotFoundException.ErrorType.BranchIdMisMatch,
                    $"Не найдена ветка с айди {branchId}"
                );

            branch.Type = CntAccessType.Convert(type);

            await ChartRepo.UpdateBranchByIdAsync(branchId, branch);
        }


        // Изменение атрибутов таблицы Station
        public async Task UpdateStationTitle(string title, int stationId)
        {
            Station station = await ChartRepo.GetStationByIdAsync(stationId) ??
                throw new NotFoundException(
                    NotFoundException.ErrorType.StationIdMisMatch,
                    $"Не найдена станция с айди {stationId}"
                );

            station.Title = title;

            await ChartRepo.UpdateStationByIdAsync(stationId, station);
        }

        public async Task UpdateStationOccupancy(int occupancy, int stationId)
        {
            Station station = await ChartRepo.GetStationByIdAsync(stationId) ??
                throw new NotFoundException(
                    NotFoundException.ErrorType.StationIdMisMatch,
                    $"Не найдена станция с айди {stationId}"
                );

            station.Occupancy = occupancy;

            await ChartRepo.UpdateStationByIdAsync(stationId, station);
        }

        public async Task UpdateStationAccessType(AccessTypeDTO type, int stationId)
        {
            Station station = await ChartRepo.GetStationByIdAsync(stationId) ??
                throw new NotFoundException(
                    NotFoundException.ErrorType.StationIdMisMatch,
                    $"Не найдена станция с айди {stationId}"
                );

            station.Type = CntAccessType.Convert(type);

            await ChartRepo.UpdateStationByIdAsync(stationId, station);
        }

        public async Task UpdateStationOpenTime(string time, int stationId)
        {
            Station station = await ChartRepo.GetStationByIdAsync(stationId) ??
                throw new NotFoundException(
                    NotFoundException.ErrorType.StationIdMisMatch,
                    $"Не найдена станция с айди {stationId}"
                );

            station.OpenTime = TimeConverter.FromString(time);

            await ChartRepo.UpdateStationByIdAsync(stationId, station);
        }

        public async Task UpdateStationCloseTime(string time, int stationId)
        {
            Station station = await ChartRepo.GetStationByIdAsync(stationId) ??
                throw new NotFoundException(
                    NotFoundException.ErrorType.StationIdMisMatch,
                    $"Не найдена станция с айди {stationId}"
                );

            station.CloseTime = TimeConverter.FromString(time);

            await ChartRepo.UpdateStationByIdAsync(stationId, station);
        }


        // Изменение атрибутов таблицы Transition
        public async Task UpdateTransitionOccupancy(int occupancy, int transitionId)
        {
            Transition transition = await ChartRepo.GetTransitionByIdAsync(transitionId) ??
                throw new NotFoundException(
                    NotFoundException.ErrorType.TransitionIdMisMatch,
                    $"Не найден переход с айди {transitionId}"
                );

            transition.Occupancy = occupancy;

            await ChartRepo.UpdateTransitionByIdAsync(transitionId, transition);
        }

        public async Task UpdateTransitionAccessType(AccessTypeDTO type, int transitionId)
        {
            Transition transition = await ChartRepo.GetTransitionByIdAsync(transitionId) ??
                throw new NotFoundException(
                    NotFoundException.ErrorType.TransitionIdMisMatch,
                    $"Не найден переход с айди {transitionId}"
                );

            transition.Type = CntAccessType.Convert(type);

            await ChartRepo.UpdateTransitionByIdAsync(transitionId, transition);
        }

        public async Task UpdateTransitionDuration(string time, int transitionId)
        {
            Transition transition = await ChartRepo.GetTransitionByIdAsync(transitionId) ??
                throw new NotFoundException(
                    NotFoundException.ErrorType.TransitionIdMisMatch,
                    $"Не найден переход с айди {transitionId}"
                );

            transition.Duration = TimeConverter.FromString(time);

            await ChartRepo.UpdateTransitionByIdAsync(transitionId, transition);
        }

        public async Task UpdateTransitionOpenTime(string time, int transitionId)
        {
            Transition transition = await ChartRepo.GetTransitionByIdAsync(transitionId) ??
                throw new NotFoundException(
                    NotFoundException.ErrorType.TransitionIdMisMatch,
                    $"Не найден переход с айди {transitionId}"
                );

            transition.OpenTime = TimeConverter.FromString(time);

            await ChartRepo.UpdateTransitionByIdAsync(transitionId, transition);
        }

        public async Task UpdateTransitionCloseTime(string time, int transitionId)
        {
            Transition transition = await ChartRepo.GetTransitionByIdAsync(transitionId) ??
                throw new NotFoundException(
                    NotFoundException.ErrorType.TransitionIdMisMatch,
                    $"Не найден переход с айди {transitionId}"
                );

            transition.CloseTime = TimeConverter.FromString(time);

            await ChartRepo.UpdateTransitionByIdAsync(transitionId, transition);
        }
    }
}