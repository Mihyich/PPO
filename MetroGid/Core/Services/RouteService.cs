using MetroGid.Controllers.Utility.Interfaces;
using MetroGid.Core.Exceptions.Classification;
using MetroGid.Core.Exceptions.Concrete;
using MetroGid.Core.Exceptions.Interfaces;
using MetroGid.Core.Exceptions.Super;
using MCMC = MetroGid.Core.Models.Concrete;
using MCMA = MetroGid.Core.Models.Advanced;
using MetroGid.Core.Utility.Builders;
using MetroGid.Core.Utility.Directors;
using MetroGid.Core.Utility.Strategies;
using MetroGid.Core.Utility.Validators.Handlers;
using MetroGid.Core.Interfaces;

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

    private async Task<MCMA.IdRow> GetClientIdAsync(string login, string password, string mail) =>
        await Handler.SnapAsync(
            async () =>
            {
                MCMA.IdRow clientIdRow = await _clientRepo.GetIdByCredentialsAsync(login, password);

                if (clientIdRow.id == 0)
                    throw new DataBaseException(
                        $"Пользователь '{login}' с почтой '{mail}' не найден",
                        ExceptionType.Warning,
                        ExceptionReason.NotFound
                    );

                return clientIdRow;
            }, Logger
        );

    private async Task<MCMC.Chart?> GetChartAsync(string city, string chartTitle)
    {
        string? jsonContent = await Handler.SnapAsync(
            async () =>
            {
                MCMA.FileRow? jsonRow = await _chartRepo.GetChartJsonByCredentialsAsync(city, chartTitle);

                if (jsonRow == null)
                    throw new DataBaseException(
                        $"Json схема \"{chartTitle}\" для города \"{city}\" не найдена",
                        ExceptionType.Warning,
                        ExceptionReason.NotFound
                    );
                return jsonRow.content;
            }, Logger
        );

        MCMC.Chart? chart = null;

        if (jsonContent != null)
        {
            BuilderChart builder = new(DomainAttribsValidator, DomainReferentialityValidator);
            DirectorChartJson director = new(builder, jsonContent);
            chart = director.Construct();
        }

        return chart;
    }

    private MCMC.Station? FindStationAsync(MCMC.Chart chart, string branchTitle, string stationTitle)
    {
        MCMC.Station? station = Handler.Snap(
            () =>
            {
                MCMC.Station? s = chart.GetStation(branchTitle, stationTitle);

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

    private MCMC.Route? SearchRouteProcess(MCMC.Chart chart, MCMC.Station src, MCMC.Station dst, TimeOnly startTime)
    {
        StrategySearchRouteBase searcher = new StrategySearchRouteBFS();
        MCMC.Route? route = chart.Search(src, dst, startTime, searcher);

        if (route != null)
        {
            route.Validate(DomainAttribsValidator);
            route.Validate(DomainReferentialityValidator);
        }

        return route;
    }

    public async Task<MCMC.Route?> SearchRouteAsync(
        string city, string chartTitle,
        string branchSrcTitle, string stationSrcTitle,
        string branchDstTitle, string stationDstTitle,
        TimeOnly startTime)
    {
        MCMC.Chart? chart = await GetChartAsync(city, chartTitle);
        MCMC.Station? src = null;
        MCMC.Station? dst = null;
        MCMC.Route? route = null;

        if (chart != null &&
            (src = FindStationAsync(chart, branchSrcTitle, stationSrcTitle)) != null &&
            (dst = FindStationAsync(chart, branchDstTitle, stationDstTitle)) != null
        )
        {
            route = SearchRouteProcess(chart, src, dst, startTime);
        }

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

    public async Task<MCMC.Route?> GetSavedChart(
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
        );
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
