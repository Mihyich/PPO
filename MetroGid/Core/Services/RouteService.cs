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
using MetroGid.Core.Interfaces;

namespace MetroGid.Core.Services;

public class RouteService(
    IChartRepository chartRepo, IRouteRepository routeRepo,
    SuperHandlerException handler, IExceptionVisitor? logger = null
) : IRouteService
{
    private readonly IChartRepository ChartRepo = chartRepo;
    private readonly IRouteRepository RouteRepo = routeRepo;

    private readonly SuperHandlerException Handler = handler;
    private readonly IExceptionVisitor? Logger = logger;

    private readonly ThrowableDomainAttribsValidator DomainAttribsValidator = new(handler, logger);
    private readonly ThrowableDomainReferentialityValidator DomainReferentialityValidator = new(handler, logger);

    private async Task<Chart> LoadChartAsync(string city, string chartTitle)
    {
        BuilderChart builder = new(DomainAttribsValidator, DomainReferentialityValidator);
        DirectorChartJson director = new(
            builder,
            await ChartRepo.GetChartJsonAsync(city, chartTitle)
        );

        StrategySearchRouteBase searcher = new StrategySearchRouteBFS();
        Chart chart = director.Construct();

        chart.Searcher = searcher;

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
        Route route = chart.Search(src, dst, startTime);

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

    public Task SaveRoute(int clientId, RouteDTO route, int chartId) =>
        RouteRepo.AddAsync(DtoDomainConverter.Convert(route), clientId, chartId);

    public Task<List<RouteDTO>> LookForSavedRoutesInChart(int clientId, int chartId)
    {
        throw new NotImplementedException();
    }
}