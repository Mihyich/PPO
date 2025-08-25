using MetroGid.Controllers.Utility.DTO;
using MetroGid.Controllers.Utility.Interfaces;
using MetroGid.Core.Converters;
using MetroGid.Core.Interfaces;
using MetroGid.Core.Models.Concrete;
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

    private async Task<int> GetChartIdAsync(string cityTitle, string chartTitle) =>
        await Handler.SnapAsync(
            async () =>
            {
                int chartId = await ChartRepo.GetChartIdAsync(cityTitle, chartTitle);

                if (chartId == 0)
                    throw new DataBaseException(
                        $"Схема '{chartTitle}' в городе '{cityTitle}' не найдена",
                        ExceptionType.Warning,
                        ExceptionReason.NotFound
                    );

                return chartId;
            }, Logger
        );

    private async Task<int> GetBranchIdAsync(string cityTitle, string chartTitle, string branchTitle)
    {
        int chartId = await GetChartIdAsync(cityTitle, chartTitle);

        return await Handler.SnapAsync(
            async () =>
            {
                int branchId = await ChartRepo.GetBranchIdAsync(branchTitle, chartId);

                if (branchId == 0)
                    throw new DataBaseException(
                        $"Ветка '{branchTitle}' не найдена",
                        ExceptionType.Warning,
                        ExceptionReason.NotFound
                    );

                return branchId;
            }, Logger
        );
    }

    private async Task<int> GetStationIdAsync(
        string cityTitle, string chartTitle,
        string branchTitle, string stationTitle)
    {
        int branchId = await GetBranchIdAsync(cityTitle, chartTitle, branchTitle);

        return await Handler.SnapAsync(
            async () =>
            {
                int stationId = await ChartRepo.GetStationIdAsync(stationTitle, branchId);

                if (stationId == 0)
                    throw new DataBaseException(
                        $"Станция '{stationTitle}' не найдена",
                        ExceptionType.Warning,
                        ExceptionReason.NotFound
                    );

                return stationId;
            }, Logger
        );
    }

    private async Task<int> GetRailwayIdAsync(
        string cityTitle, string chartTitle,
        string branchTitle,
        string fromStationTitle, string toStationTitle)
    {
        int fromStationId = await GetStationIdAsync(
            cityTitle, chartTitle,
            branchTitle, fromStationTitle
        );

        int toStationId = await GetStationIdAsync(
            cityTitle, chartTitle,
            branchTitle, toStationTitle
        );

        return await Handler.SnapAsync(
            async () =>
            {
                int railwayId = await ChartRepo.GetRailwayIdAsync(fromStationId, toStationId);

                if (railwayId == 0)
                    throw new DataBaseException(
                        $"Переезд с айди '{railwayId}' не найден",
                        ExceptionType.Warning,
                        ExceptionReason.NotFound
                    );

                return railwayId;
            }, Logger
        );
    }

    private async Task<int> GetTransitionIdAsync(
        string cityTitle, string chartTitle,
        string fromBranchTitle, string fromStationTitle,
        string toBranchTitle, string toStationTitle)
    {
        int fromStationId = await GetStationIdAsync(
            cityTitle, chartTitle,
            fromBranchTitle, fromStationTitle
        );

        int toStationId = await GetStationIdAsync(
            cityTitle, chartTitle,
            toBranchTitle, toStationTitle
        );

        return await Handler.SnapAsync(
            async () =>
            {
                int transitionId = await ChartRepo.GetTransitionIdAsync(fromStationId, toStationId);

                if (transitionId == 0)
                    throw new DataBaseException(
                        $"Переход с айди '{transitionId}' не найден",
                        ExceptionType.Warning,
                        ExceptionReason.NotFound
                    );

                return transitionId;
            }, Logger
        );
    }


    public async Task<ChartDTO?> GetChartAsync(string cityTitle, string chartTitle)
    {
        int chartId = await GetChartIdAsync(cityTitle, chartTitle);

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

    public async Task<List<ValueTuple<string, string>>> GetChartsCitiesTitlesAsync() =>
        await ChartRepo.GetAllChartCityTitleAsync();


    public async Task<BranchDTO?> GetBranchAsync(string cityTitle, string chartTitle, string branchTitle)
    {
        int branchId = await GetBranchIdAsync(cityTitle, chartTitle, branchTitle);

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

    public async Task<List<string>> GetChartBranchTitlesAsync(int chartId) =>
        await ChartRepo.GetAllChartBranchTitleAsync(chartId);

    public async Task<StationDTO?> GetStationAsync(
        string cityTitle, string chartTitle,
        string branchTitle, string stationTitle)
    {
        int stationId = await GetStationIdAsync(
            cityTitle, chartTitle,
            branchTitle, stationTitle
        );

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

    public async Task<List<string>> GetBranchStationTitlesAsync(string cityTitle, string chartTitle, string branchTitle)
    {
        int branchId = await GetBranchIdAsync(cityTitle, chartTitle, branchTitle);
        return await ChartRepo.GetAllBranchStationTitleAsync(branchId);
    }

    public async Task<TransitionDTO?> GetTransitionAsync(
        string cityTitle, string chartTitle,
        string fromBranchTitle, string fromStationTitle,
        string toBranchTitle, string toStationTitle)
    {
        int transitionId = await GetTransitionIdAsync(
            cityTitle, chartTitle,
            fromBranchTitle, fromStationTitle,
            toBranchTitle, toStationTitle
        );

        Transition? transition = await Handler.SnapAsync(
            async () =>
            {
                return await ChartRepo.GetTransitionByIdAsync(transitionId) ??
                    throw new DataBaseException(
                        $"Переход с айди {transitionId} не найден",
                        ExceptionType.Warning,
                        ExceptionReason.NotFound
                    );
            }, Logger
        );

        return transition != null ? DomainDtoConverter.Convert(transition) : null;
    }

    public async Task<RailwayDTO?> GetRailwayAsync(
        string cityTitle, string chartTitle,
        string branchTitle,
        string fromStationTitle, string toStationTitle)
    {
        int railwayId = await GetRailwayIdAsync(
            cityTitle, chartTitle,
            branchTitle,
            fromStationTitle, toStationTitle
        );

        Railway? railway = await Handler.SnapAsync(
            async () =>
            {
                return await ChartRepo.GetRailwayByIdAsync(railwayId) ??
                    throw new DataBaseException(
                        $"Переезд с айди {railwayId} не найден",
                        ExceptionType.Warning,
                        ExceptionReason.NotFound
                    );
            }, Logger
        );

        return railway != null ? DomainDtoConverter.Convert(railway) : null;
    }

    public async Task<int> UpdateChartAsync(
        RoleTypeDTO role,
        string chartCity, string chartTitle,
        ChartDTO chart
    )
    {
        if (DtoDomainConverter.Convert(role) != RoleType.DUTY)
            return 0;

        int chartId = await ChartRepo.GetChartIdAsync(chartCity, chartTitle);

        return await ChartRepo.UpdateChartByIdAsync(chartId, DtoDomainConverter.Convert(chart));
    }

    public async Task<int> UpdateBranchAsync(
        RoleTypeDTO role,
        string chartCity, string chartTitle,
        string branchTitle,
        BranchDTO branch
    )
    {
        if (DtoDomainConverter.Convert(role) != RoleType.DUTY)
            return 0;

        int branchId = await GetBranchIdAsync(chartCity, chartTitle, branchTitle);

        return await ChartRepo.UpdateBranchByIdAsync(branchId, DtoDomainConverter.Convert(branch));
    }

    public async Task<int> UpdateStationAsync(
        RoleTypeDTO role,
        string chartCity, string chartTitle,
        string branchTitle, string stationTitle,
        StationDTO station)
    {
        if (DtoDomainConverter.Convert(role) != RoleType.DUTY)
            return 0;

        int stationId = await GetStationIdAsync(
            chartCity, chartTitle,
            branchTitle, stationTitle
        );

        return await ChartRepo.UpdateStationByIdAsync(stationId, DtoDomainConverter.Convert(station));
    }

    public async Task<int> UpdateRailwayAsync(
        RoleTypeDTO role,
        string chartCity, string chartTitle,
        string BranchTitle,
        string fromStationTitle, string toStationTitle,
        RailwayDTO railway)
    {
        if (DtoDomainConverter.Convert(role) != RoleType.DUTY)
            return 0;

        int railwayId = await GetRailwayIdAsync(
            chartCity, chartTitle,
            BranchTitle,
            fromStationTitle, toStationTitle
        );

        return await ChartRepo.UpdateRailwayByIdAsync(railwayId, DtoDomainConverter.Convert(railway));
    }

    public async Task<int> UpdateTransitionAsync(
        RoleTypeDTO role,
        string chartCity, string chartTitle,
        string fromBranchTitle, string fromStationTitle,
        string toBranchTitle, string toStationTitle,
        TransitionDTO transition)
    {
        if (DtoDomainConverter.Convert(role) != RoleType.DUTY)
            return 0;

        int transitionId = await GetTransitionIdAsync(
            chartCity, chartTitle,
            fromBranchTitle, fromStationTitle,
            toBranchTitle, toStationTitle
        );

        return await ChartRepo.UpdateTransitionByIdAsync(transitionId, DtoDomainConverter.Convert(transition));
    }
}