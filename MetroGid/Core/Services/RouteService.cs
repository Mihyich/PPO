using MetroGid.Controllers.DTO;
using MetroGid.Controllers.Interfaces;
using MetroGid.Core.Exceptions.Classification;
using MetroGid.Core.Exceptions.Concrete;
using MetroGid.Core.Exceptions.Handlers;
using MetroGid.Core.Exceptions.Loggers;
using MetroGid.Core.Models.Concrete;
using MetroGid.Core.Utilities;
using MetroGid.Core.Utilities.Builders;
using MetroGid.Core.Utilities.Directors;
using MetroGid.Core.Utilities.Strategies;
using MetroGid.Core.Utilities.Validators.Handlers;
using MetroGid.DBA.Interfaces;

namespace MetroGid.Core.Services
{
    public class RouteService(
        IChartRepository chartRepo,
        IRouteRepository routeRepo) : IRouteService
    {
        private readonly IChartRepository ChartRepo = chartRepo;
        private readonly IRouteRepository RouteRepo = routeRepo;

        public async Task<RouteDTO> SearchRoute(
            string city, string chartTitle,
            string branchSrcTitle, string stationSrcTitle,
            string branchDstTitle, string stationDstTitle,
            TimeOnly startTime)
        {
            WarningHandlerException handler = new();
            ExceptionMessenger logger = new();

            ThrowableDomainAttribsValidator DomainAttribsValidator = new(handler, logger);
            ThrowableDomainReferentialityValidator DomainReferentialityValidator = new(handler, logger);

            BuilderChart builder = new(DomainAttribsValidator, DomainReferentialityValidator);
            DirectorChartJson director = new(
                builder,
                await ChartRepo.GetChartJsonByIdAsync(await ChartRepo.GetChartIdAsync(city, chartTitle))
            );

            StrategySearchRouteBase searcher = new StrategySearchRouteBFS();
            Chart chart = director.Construct();

            chart.Searcher = searcher;

            Station src = chart.GetStation(branchSrcTitle, stationSrcTitle) ??
                throw new ServiceRouteException(
                    $"В схеме '{chart.Title}' в городе '{chart.City}' не найдена станция '{stationSrcTitle}' ветки '{branchSrcTitle}'",
                    ExceptionType.Error,
                    ExceptionReason.NotFound
                );

            Station dst = chart.GetStation(branchDstTitle, stationDstTitle) ??
                throw new ServiceRouteException(
                    $"В схеме '{chart.Title}' в городе '{chart.City}' не найдена станция '{stationDstTitle}' ветки '{branchDstTitle}'",
                    ExceptionType.Error,
                    ExceptionReason.NotFound
                );

            return CntRoute.Convert(chart.Search(src, dst, startTime));
        }

        public Task SaveRoute(int clientId, RouteDTO route, int chartId) =>
            RouteRepo.AddAsync(CntRoute.Convert(route), clientId, chartId);

        public Task<List<RouteDTO>> LookForSavedRoutesInChart(int clientId, int chartId)
        {
            throw new NotImplementedException();
        }
    }
}