using MetroGid.Controllers.Utility.DTO;
using MetroGid.Controllers.Utility.Interfaces;
using MetroGid.Core.Converters;
using MetroGid.Core.Exceptions.Classification;
using MetroGid.Core.Exceptions.Concrete;
using MetroGid.Core.Exceptions.Interfaces;
using MetroGid.Core.Exceptions.Super;
using MetroGid.Core.Models.Concrete;
using MetroGid.Core.Utility.Builders;
using MetroGid.Core.Utility.Directors;
using MetroGid.Core.Utility.Strategies;
using MetroGid.Core.Utility.Validators.Handlers;
using MetroGid.Core.Interfaces;
using MetroGid.Core.Models.Types;

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
                int clientId = await ClientRepo.GetIdByCredentialsAsync(login, password, mail);

                if (clientId == 0)
                    throw new DataBaseException(
                        $"Пользователь '{login}' с почтой '{mail}' не найден",
                        ExceptionType.Warning,
                        ExceptionReason.NotFound
                    );
                
                return clientId;
            }, Logger
        );

    private async Task<Chart?> GetChartAsync(string city, string chartTitle)
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

    private Station? FindStationAsync(Chart chart, string branchTitle, string stationTitle)
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
    
    public async Task<RouteDTO?> SearchRouteAsync(
        string city, string chartTitle,
        string branchSrcTitle, string stationSrcTitle,
        string branchDstTitle, string stationDstTitle,
        TimeOnly startTime)
    {
        Chart? chart = await GetChartAsync(city, chartTitle);
        Station? src = null;
        Station? dst = null;
        RouteDTO? routeDTO = null;
        Route? route = null;

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

    public async Task<int> SaveRouteAsync(ClientDTO client, RouteDTO route, int chartId)
    {
        RoleType role = DtoDomainConverter.Convert(client.Role);

        if (role != RoleType.SIGNED && role != RoleType.DUTY)
            return 0;

        int clientId = await GetClientIdAsync(client.Login, client.Password, client.Mail);

        return await RouteRepo.AddAsync(clientId, chartId, DtoRouteJsonConverter.Convert(route));
    }

    public async Task<List<string>> LookForSavedRoutesInChartAsync(ClientDTO client, int chartId)
    {
        RoleType role = DtoDomainConverter.Convert(client.Role);

        if (role != RoleType.SIGNED && role != RoleType.DUTY)
            return new List<string>();

        int clientId = await GetClientIdAsync(client.Login, client.Password, client.Mail);

        return await RouteRepo.GetAllForClientOfChartIdAsync(clientId, chartId);
    }
}