using MetroGid.Controllers.Utility.Interfaces;
using MetroGid.Core.Exceptions.Interfaces;
using MetroGid.Core.Exceptions.Super;
using MCMC = MetroGid.Core.Models.Concrete;
using MCMA = MetroGid.Core.Models.Advanced;
using MetroGid.Core.Utility.Builders;
using MetroGid.Core.Utility.Directors;
using MetroGid.Core.Utility.Strategies;
using MetroGid.Core.Utility.Validators.Handlers;
using MetroGid.Core.Interfaces;
using MetroGid.Core.Exceptions.Truistic;

namespace MetroGid.Core.Services;

public class RouteService(
    IChartRepository chartRepo, IClientRepository clientRepo, IRouteRepository routeRepo,
    ThrowableDomainAttribsValidator domainAttribsValidator,
    ThrowableDomainReferentialityValidator domainReferentialityValidator,
    SuperExceptionHandler handler,
    IExceptionVisitor? logger = null
) : IRouteService
{
    private readonly IChartRepository _chartRepo = chartRepo;
    private readonly IClientRepository _clientRepo = clientRepo;
    private readonly IRouteRepository _routeRepo = routeRepo;

    private readonly ThrowableDomainAttribsValidator DomainAttribsValidator = domainAttribsValidator;
    private readonly ThrowableDomainReferentialityValidator DomainReferentialityValidator = domainReferentialityValidator;

    private readonly SuperExceptionHandler Handler = handler;
    private readonly IExceptionVisitor? Logger = logger;

    private async Task<MCMA.IdRow> GetClientIdAsync(string login, string password, string mail)
    {
        MCMA.IdRow clientIdRow = await _clientRepo.GetIdByCredentialsAsync(login, password);

        if (clientIdRow.id == 0)
            throw new UnknownClientCredentialsException(login, password);

        return clientIdRow;
    }

    private async Task<MCMC.Chart> GetChartAsync(string city, string chartTitle)
    {
        MCMA.FileRow jsonRow = await _chartRepo.GetChartJsonByCredentialsAsync(city, chartTitle) ??
            throw new UnknownChartCredentialsException(chartTitle, city);

        BuilderChart builder = new(DomainAttribsValidator, DomainReferentialityValidator);
        DirectorChartJson director = new(builder, jsonRow.content);
        return director.Construct();
    }

    private MCMC.Station FindStationAsync(MCMC.Chart chart, string branchTitle, string stationTitle) =>
        chart.GetStation(branchTitle, stationTitle) ??
            throw new UnknownStationCredentialsException(stationTitle, -1);

    private MCMC.Route SearchRouteProcess(MCMC.Chart chart, MCMC.Station src, MCMC.Station dst, TimeOnly startTime)
    {
        StrategySearchRouteBase searcher = new StrategySearchRouteBFS();

        MCMC.Route route = chart.Search(src, dst, startTime, searcher) ??
            throw new RouteNotFoundException(
                chart.Title, chart.City,
                src.Branch?.Title ?? "?", src.Title,
                dst.Branch?.Title ?? "?", dst.Title,
                startTime
            );

        route.Validate(DomainAttribsValidator);
        route.Validate(DomainReferentialityValidator);

        return route;
    }

    public async Task<MCMC.Route> SearchRouteAsync(
        string city, string chartTitle,
        string branchSrcTitle, string stationSrcTitle,
        string branchDstTitle, string stationDstTitle,
        TimeOnly startTime)
    {
        MCMC.Chart chart = await GetChartAsync(city, chartTitle);
        MCMC.Station src = FindStationAsync(chart, branchSrcTitle, stationSrcTitle);
        MCMC.Station dst = FindStationAsync(chart, branchDstTitle, stationDstTitle);
        MCMC.Route route = SearchRouteProcess(chart, src, dst, startTime);
        return route;
    }

    public async Task<MCMA.IdRow> SaveRouteAsync(
        int clientId,
        MCMC.Route route
    )
    {
        MCMA.IdRow chartIdRow = await _chartRepo.GetChartIdAsync(
            route.Chart?.City ?? "",
            route.Chart?.Title ?? ""
        );

        return await _routeRepo.AddAsync(
            clientId,
            chartIdRow.id,
            route
        );
    }

    public async Task<MCMA.TitlesRow> GetSavedChartRoutesTitles(
        int clientId,
        string city,
        string chartTitle
    )
    {
        MCMA.IdRow chartIdRow = await _chartRepo.GetChartIdAsync(
            city,
            chartTitle
        );

        return await _routeRepo.GetAllForClientOfChartIdAsync(
            clientId,
            chartIdRow.id
        );
    }

    public async Task<MCMC.Route> GetSavedChart(
        int clientId,
        string city,
        string chartTitle,
        string title
    )
    {
        MCMA.IdRow chartIdRow = await _chartRepo.GetChartIdAsync(
            city,
            chartTitle
        );

        return await _routeRepo.GetChartRouteOfClient(
            clientId,
            chartIdRow.id,
            title
        ) ??
        throw new SavedRouteNotFoundException(clientId, chartIdRow.id, title);
    }

    public async Task<MCMA.DeletedRowCount> DeleteAsync(
        int clientId,
        string title
    )
    {
        MCMA.IdRow chartIdRow = await _routeRepo.GetIdAsync(
            title,
            clientId
        );

        return await _routeRepo.DeleteAsync(chartIdRow.id);
    }
}
