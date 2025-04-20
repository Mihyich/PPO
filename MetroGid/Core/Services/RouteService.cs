using MetroGid.Controllers.DTO;
using MetroGid.Controllers.Interfaces;
using MetroGid.Core.Models;
using MetroGid.Core.Utilities;
using MetroGid.DBA.Interfaces;

namespace MetroGid.Core.Services
{
    public class RouteService(IChartRepository chartRepo, IRouteRepository routeRepo) : IRouteService
    {
        private readonly IChartRepository ChartRepo = chartRepo;
        private readonly IRouteRepository RouteRepo = routeRepo;

        public async Task<RouteDTO> SearchRoute(
            string city, string chartTitle,
            string branchSrcTitle, string stationSrcTitle,
            string branchDstTitle, string stationDstTitle)
        {
            BuilderChart builder = new();

            DirectorChartJson director = new(
                builder,
                await ChartRepo.GetChartJsonByIdAsync(
                    await ChartRepo.GetChartIdAsync(city, chartTitle))
            );

            Chart chart = director.Construct();
            chart.Searcher = new StrategySearchRouteDijkstra();
            Station? src = chart?.GetStation(branchSrcTitle, stationSrcTitle);
            Station? dst = chart?.GetStation(branchDstTitle, stationDstTitle);

            return CntRoute.Convert(chart?.Search(src, dst));
        }

        public Task SaveRoute(int clientId, RouteDTO route, int chartId) =>
            RouteRepo.AddAsync(CntRoute.Convert(route), clientId, chartId);

        public Task<List<RouteDTO>> LookForSavedRoutesInChart(int clientId, int chartId)
        {
            throw new NotImplementedException();
        }
    }
}