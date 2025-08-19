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
using MetroGid.Core.Models.Types;

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



    public async Task<int> UpdateChart(RoleTypeDTO role, int chartId, ChartDTO chart) =>
        DtoDomainConverter.Convert(role) == RoleType.DUTY ?
        await ChartRepo.UpdateChartByIdAsync(chartId, DtoDomainConverter.Convert(chart)) :
        0;

    public async Task<int> UpdateBranch(RoleTypeDTO role, int branchId, BranchDTO branch) =>
        DtoDomainConverter.Convert(role) == RoleType.DUTY ?
        await ChartRepo.UpdateBranchByIdAsync(branchId, DtoDomainConverter.Convert(branch)) :
        0;

    public async Task<int> UpdateStation(RoleTypeDTO role, int stationId, StationDTO station) =>
        DtoDomainConverter.Convert(role) == RoleType.DUTY ?
        await ChartRepo.UpdateStationByIdAsync(stationId, DtoDomainConverter.Convert(station)) :
        0;

    public async Task<int> UpdateTransition(RoleTypeDTO role, int transitionId, TransitionDTO transition) =>
        DtoDomainConverter.Convert(role) == RoleType.DUTY ?
        await ChartRepo.UpdateTransitionByIdAsync(transitionId, DtoDomainConverter.Convert(transition)) :
        0;
}