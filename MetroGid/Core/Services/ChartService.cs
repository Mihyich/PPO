using MetroGid.Controllers.DTO;
using MetroGid.Controllers.Interfaces;
using MetroGid.Core.Converters;
using MetroGid.Core.Interfaces;
using MetroGid.Core.Models.Concrete;
using MetroGid.Core.Interfaces;
using MetroGid.Core.Exceptions.Super;
using MetroGid.Core.Exceptions.Interfaces;
using MetroGid.Core.Exceptions.Concrete;
using MetroGid.Core.Exceptions.Classification;

namespace MetroGid.Core.Services;

public class ChartService(
    IChartRepository chartRepo,
    SuperExceptionHandler handler, IExceptionVisitor? logger = null
) : IChartService
{
    private readonly IChartRepository ChartRepo = chartRepo;

    private readonly SuperExceptionHandler Handler = handler;
    private readonly IExceptionVisitor? Logger = logger;

    public async Task<ChartDTO?> GetChart(int chartId)
    {
        Chart? chart = await Handler.SnapAsync(
            async () =>
            {
                return await ChartRepo.GetChartWeakByIdAsync(chartId) ??
                    throw new DataBaseException(
                        $"Схема с айди {chartId} не найдена",
                        ExceptionType.Warning,
                        ExceptionReason.NotFound
                    );
            }, Logger
        );

        return chart != null ? DomainDtoConverter.Convert(chart) : null;
    }

    public async Task<int> GetChartId(string city, string title) =>
        await ChartRepo.GetChartIdAsync(city, title);

    public async Task<(string, string)> GetChartsCitiesTitles() =>
        await ChartRepo.GetAllChartCityTitleAsync();



    public async Task<BranchDTO?> GetBranch(int branchId)
    {
        Branch? branch = await Handler.SnapAsync(
            async () =>
            {
                return await ChartRepo.GetBranchWeakByIdAsync(branchId) ??
                    throw new DataBaseException(
                        $"Ветка с айди {branchId} не найдена",
                        ExceptionType.Warning,
                        ExceptionReason.NotFound
                    );
            }, Logger
        );

        return branch != null ? DomainDtoConverter.Convert(branch) : null;
    }

    public async Task<int> GetChartBranchId(string title, int chartId) =>
        await ChartRepo.GetBranchIdAsync(title, chartId);

    public async Task<List<string>> GetChartBranchTitles(int chartId) =>
        await ChartRepo.GetAllChartBranchTitleAsync(chartId);



    public async Task<StationDTO?> GetStation(int stationId)
    {
        Station? station = await Handler.SnapAsync(
            async () =>
            {
                return await ChartRepo.GetStationWeakByIdAsync(stationId) ??
                    throw new DataBaseException(
                        $"Станция с айди {stationId} не найдена",
                        ExceptionType.Warning,
                        ExceptionReason.NotFound
                    );
            }, Logger
        );

        return station != null ? DomainDtoConverter.Convert(station) : null;
    }

    public async Task<int> GetBranchStationId(string title, int branchId) =>
        await ChartRepo.GetStationIdAsync(title, branchId);

    public async Task<List<string>> GetBranchStationTitles(int branchId) =>
        await ChartRepo.GetAllBranchStationTitleAsync(branchId);



    public async Task<int> UpdateChart(int chartId, ChartDTO chart) =>
        await ChartRepo.UpdateChartByIdAsync(chartId, DtoDomainConverter.Convert(chart));

    public async Task<int> UpdateBranch(int branchId, BranchDTO branch) =>
        await ChartRepo.UpdateBranchByIdAsync(branchId, DtoDomainConverter.Convert(branch));

    public async Task<int> UpdateStation(int stationId, StationDTO station) =>
        await ChartRepo.UpdateStationByIdAsync(stationId, DtoDomainConverter.Convert(station));

    public async Task<int> UpdateTransition(int transitionId, TransitionDTO transition) =>
        await ChartRepo.UpdateTransitionByIdAsync(transitionId, DtoDomainConverter.Convert(transition));
}