using System.Linq;
using MetroGid.Controllers.DTO;
using MetroGid.Controllers.Interfaces;
using MetroGid.Core.Converters;
using MetroGid.Core.Exceptions.Classification;
using MetroGid.Core.Exceptions.Concrete;
using MetroGid.Core.Exceptions.Interfaces;
using MetroGid.Core.Exceptions.Super;
using MetroGid.Core.Models.Concrete;
using MetroGid.Core.Models.Types;
using MetroGid.Core.Utilities.Builders;
using MetroGid.Core.Utilities.Directors;
using MetroGid.Core.Utilities.Strategies;
using MetroGid.Core.Utilities.Validators.Handlers;
using MetroGid.DBA.Interfaces;

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

    private async Task<Chart> LoadChartAsync(string city, string chartTitle)
    {
        BuilderChart builder = new(DomainAttribsValidator, DomainReferentialityValidator);
        DirectorChartJson director = new(
            builder,
            await ChartRepo.GetChartJsonAsync(city, chartTitle)
        );

        Chart chart = director.Construct();

        return chart;
    }

    private Station FindStation(Chart chart, string branchTitle, string stationTitle)
    {
        Station? station = chart.GetStation(branchTitle, stationTitle);
        
        if (station == null)
        {
            ServiceRouteException ex = new(
                $"В схеме '{chart.Title}' в городе '{chart.City}' не найдена станция '{stationTitle}' ветки '{branchTitle}'",
                ExceptionType.Error,
                ExceptionReason.NotFound
            );
            
            Handler.Snap(() => throw ex, Logger);

            station = new("Исключение из FindStation()", 0, AccessType.INACCESSIBLE, new TimeOnly(0, 0), new TimeOnly(0, 0));
        }

        return station;
    }

    private Route SearchRouteProcess(Chart chart, Station src, Station dst, TimeOnly startTime)
    {
        StrategySearchRouteBase searcher = new StrategySearchRouteBFS();
        Route route = chart.Search(src, dst, startTime, searcher);

        if (route.Path.Count == 0)
        {
            ServiceRouteException ex = new(
                $"Маршрут не удалось найти в схеме '{chart.Title}' в городе '{chart.City}' от станции '{src.Title}' ветки '{src.Branch?.Title ?? "Неизвестно"}' до станции '{dst.Title}' ветки '{dst.Branch?.Title ?? "Неизвестно"}'",
                ExceptionType.Quiet,
                ExceptionReason.NotFound
            );

            Handler.Snap(() => throw ex, Logger);
        }
        else
        {
            route.Validate(DomainAttribsValidator);
            route.Validate(DomainReferentialityValidator);
        }

        return route;
    }
    
    public async Task<RouteDTO> SearchRoute(
        string city, string chartTitle,
        string branchSrcTitle, string stationSrcTitle,
        string branchDstTitle, string stationDstTitle,
        TimeOnly startTime)
    {
        Chart chart = await LoadChartAsync(city, chartTitle);
        Station src = FindStation(chart, branchSrcTitle, stationSrcTitle);
        Station dst = FindStation(chart, branchDstTitle, stationDstTitle);
        Route route = SearchRouteProcess(chart, src, dst, startTime);
        return DomainDtoConverter.Convert(route);
    }

    public async Task SaveRoute(int clientId, RouteDTO route, int chartId) =>
        await RouteRepo.AddAsync(DtoDomainConverter.Convert(route), clientId, chartId);

    public async Task<List<RouteDTO>> LookForSavedRoutesInChart(int clientId, int chartId) =>
        (await RouteRepo
            .GetAllForClientOfChartIdAsync(clientId, chartId))
                .ConvertAll(route => DomainDtoConverter.Convert(route));
}