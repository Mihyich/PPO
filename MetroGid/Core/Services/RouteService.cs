using MetroGid.Controllers.DTO;
using MetroGid.Controllers.Interfaces;
using MetroGid.Core.Converters;
using MetroGid.Core.Exceptions.Classification;
using MetroGid.Core.Exceptions.Concrete;
using MetroGid.Core.Exceptions.Interfaces;
using MetroGid.Core.Exceptions.Super;
using MetroGid.Core.Models.Concrete;
using MetroGid.Core.Utilities.Builders;
using MetroGid.Core.Utilities.Directors;
using MetroGid.Core.Utilities.Strategies;
using MetroGid.Core.Utilities.Validators.Handlers;
using MetroGid.Core.Interfaces;
using MetroGid.Core.Models.Types;

namespace MetroGid.Core.Services;

public class RouteService(
    IChartRepository chartRepo, IRouteRepository routeRepo,
    ThrowableDomainAttribsValidator domainAttribsValidator,
    ThrowableDomainReferentialityValidator domainReferentialityValidator,
    SuperExceptionHandler handler,
    IExceptionVisitor? logger = null
) : IRouteService
{
    private readonly IChartRepository ChartRepo = chartRepo;
    private readonly IRouteRepository RouteRepo = routeRepo;

    private readonly ThrowableDomainAttribsValidator DomainAttribsValidator = domainAttribsValidator;
    private readonly ThrowableDomainReferentialityValidator DomainReferentialityValidator = domainReferentialityValidator;

    private readonly SuperExceptionHandler Handler = handler;
    private readonly IExceptionVisitor? Logger = logger;

    private async Task<Chart?> LoadChartAsync(string city, string chartTitle)
    {
        string? jsonContent = await Handler.SnapAsync(
            async () =>
            {
                string? json = await ChartRepo.GetChartJsonByCredentialsAsync(city, chartTitle);

                if (json == null)
                    throw new DataBaseException(
                        $"Json схема \"{chartTitle}\" для города \"{city}\" не найдена",
                        ExceptionType.Warning,
                        ExceptionReason.NotFound
                    );
                return json;
            }, Logger
        );

        Chart? chart = null;

        if (jsonContent != null)
        {
            BuilderChart builder = new(DomainAttribsValidator, DomainReferentialityValidator);
            DirectorChartJson director = new(builder, jsonContent);
            chart = director.Construct();
        }

        return chart;
    }

    private Station? FindStation(Chart chart, string branchTitle, string stationTitle)
    {
        Station? station = Handler.Snap(
            () =>
            {
                Station? s = chart.GetStation(branchTitle, stationTitle);

                if (s == null)
                    throw new ServiceRouteException(
                        $"В схеме '{chart.Title}' для города '{chart.City}' не найдена станция '{stationTitle}' ветки '{branchTitle}'",
                        ExceptionType.Error,
                        ExceptionReason.NotFound
                    );

                return s;
            }, Logger
        );

        return station;
    }

    private Route? SearchRouteProcess(Chart chart, Station src, Station dst, TimeOnly startTime)
    {
        StrategySearchRouteBase searcher = new StrategySearchRouteBFS();
        Route? route = chart.Search(src, dst, startTime, searcher);

        if (route != null)
        {
            route.Validate(DomainAttribsValidator);
            route.Validate(DomainReferentialityValidator);
        }

        return route;
    }
    
    public async Task<RouteDTO?> SearchRoute(
        string city, string chartTitle,
        string branchSrcTitle, string stationSrcTitle,
        string branchDstTitle, string stationDstTitle,
        TimeOnly startTime)
    {
        Chart? chart = await LoadChartAsync(city, chartTitle);
        Station? src = null;
        Station? dst = null;
        RouteDTO? routeDTO = null;
        Route? route = null;

        if (chart != null &&
            (src = FindStation(chart, branchSrcTitle, stationSrcTitle)) != null &&
            (dst = FindStation(chart, branchDstTitle, stationDstTitle)) != null &&
            (route = SearchRouteProcess(chart, src, dst, startTime)) != null
        )
        {
            routeDTO = DomainDtoConverter.Convert(route);
        }

        return routeDTO;
    }

    public async Task<int> SaveRoute(RoleTypeDTO role, int clientId, RouteDTO route, int chartId)
    {
        RoleType roleType = DtoDomainConverter.Convert(role);

        return (roleType == RoleType.SIGNED || roleType == RoleType.DUTY) ?
        await RouteRepo.AddAsync(clientId, chartId, DtoDomainConverter.Convert(route)) :
        0;
    }

    public async Task<List<RouteDTO>> LookForSavedRoutesInChart(RoleTypeDTO role, int clientId, int chartId)
    {
        RoleType roleType = DtoDomainConverter.Convert(role);

        return (roleType == RoleType.SIGNED || roleType == RoleType.DUTY) ?
        (await RouteRepo
            .GetAllRouteForClientOfChartIdAsync(clientId, chartId))
                .ConvertAll(DomainDtoConverter.Convert) :
        new List<RouteDTO>();
    }
}