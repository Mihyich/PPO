using MCUD = MetroGid.Controllers.Utility.DTO.Concrete;
using MetroGid.Controllers.Utility.Interfaces;
using MetroGid.Core.Converters;
using MetroGid.Core.Exceptions.Classification;
using MetroGid.Core.Exceptions.Concrete;
using MetroGid.Core.Exceptions.Interfaces;
using MetroGid.Core.Exceptions.Super;
using MCMC = MetroGid.Core.Models.Concrete;
using MetroGid.Core.Utility.Builders;
using MetroGid.Core.Utility.Directors;
using MetroGid.Core.Utility.Strategies;
using MetroGid.Core.Utility.Validators.Handlers;
using MetroGid.Core.Interfaces;
using MCMT = MetroGid.Core.Models.Types;

namespace MetroGid.Core.Services;

public class RouteService(
    IChartRepository chartRepo, IClientRepository clientRepo, IRouteRepository routeRepo,
    ThrowableDomainAttribsValidator domainAttribsValidator,
    ThrowableDomainReferentialityValidator domainReferentialityValidator,
    SuperExceptionHandler handler,
    IExceptionVisitor? logger = null
) : IRouteService
{
    private readonly IChartRepository ChartRepo = chartRepo;
    private readonly IClientRepository ClientRepo = clientRepo;
    private readonly IRouteRepository RouteRepo = routeRepo;

    private readonly ThrowableDomainAttribsValidator DomainAttribsValidator = domainAttribsValidator;
    private readonly ThrowableDomainReferentialityValidator DomainReferentialityValidator = domainReferentialityValidator;

    private readonly SuperExceptionHandler Handler = handler;
    private readonly IExceptionVisitor? Logger = logger;

    private async Task<int> GetClientIdAsync(string login, string password, string mail) =>
        await Handler.SnapAsync(
            async () =>
            {
                int clientId = await ClientRepo.GetIdByCredentialsAsync(login, password);

                if (clientId == 0)
                    throw new DataBaseException(
                        $"Пользователь '{login}' с почтой '{mail}' не найден",
                        ExceptionType.Warning,
                        ExceptionReason.NotFound
                    );
                
                return clientId;
            }, Logger
        );

    private async Task<MCMC.Chart?> GetChartAsync(string city, string chartTitle)
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
    
    public async Task<MCUD.RouteDTO?> SearchRouteAsync(
        string city, string chartTitle,
        string branchSrcTitle, string stationSrcTitle,
        string branchDstTitle, string stationDstTitle,
        TimeOnly startTime)
    {
        MCMC.Chart? chart = await GetChartAsync(city, chartTitle);
        MCMC.Station? src = null;
        MCMC.Station? dst = null;
        MCUD.RouteDTO? routeDTO = null;
        MCMC.Route? route = null;

        if (chart != null &&
            (src = FindStationAsync(chart, branchSrcTitle, stationSrcTitle)) != null &&
            (dst = FindStationAsync(chart, branchDstTitle, stationDstTitle)) != null &&
            (route = SearchRouteProcess(chart, src, dst, startTime)) != null
        )
        {
            routeDTO = DomainDtoConverter.Convert(route);
        }

        return routeDTO;
    }

    public async Task<int> SaveRouteAsync(MCUD.ClientDTO client, MCUD.RouteDTO route, int chartId)
    {
        MCMT.RoleType role = DtoDomainConverter.Convert(client.Role);

        if (role != MCMT.RoleType.SIGNED && role != MCMT.RoleType.DUTY)
            return 0;

        int clientId = await GetClientIdAsync(client.Login, client.Password, client.Mail);

        return await RouteRepo.AddAsync(clientId, chartId, DtoRouteJsonConverter.Convert(route));
    }

    public async Task<List<string>> LookForSavedRoutesInChartAsync(MCUD.ClientDTO client, int chartId)
    {
        MCMT.RoleType role = DtoDomainConverter.Convert(client.Role);

        if (role != MCMT.RoleType.SIGNED && role != MCMT.RoleType.DUTY)
            return new List<string>();

        int clientId = await GetClientIdAsync(client.Login, client.Password, client.Mail);

        return await RouteRepo.GetAllForClientOfChartIdAsync(clientId, chartId);
    }
}