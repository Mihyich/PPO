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
using static MetroGid.Core.Converters.DtoRouteJsonConverter;

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

    public async Task<int> SaveRouteAsync(
        int clientId,
        int chartId,
        string routeJson
    ) =>
        await RouteRepo.AddAsync(clientId, chartId, routeJson);

    public async Task<List<string>> GetSavedChartRoutesTitles(
        int clientId,
        int chartId
    ) =>
        await RouteRepo.GetAllForClientOfChartIdAsync(clientId, chartId);

    public async Task<MCUD.RouteDTO?> GetSavedChart(
        int clientId,
        int chartId,
        string title
    )
    {
        string? routeJson = await RouteRepo.GetChartRouteOfClient(clientId, chartId, title);
        MCUD.RouteDTO? route = routeJson != null ? DtoRouteJsonConverter.Convert(routeJson) : null;
        List<MCUD.RouteItemDTO> path = [];

        int branchId = 0;
        int prevStationId = 0;

        if (route != null)
            foreach (var item in route.Path)
            {
                switch (item)
                {
                    case MCUD.RouteStationItemDTO routeStationItem:
                        {
                            MCUD.StationDTO cutS = routeStationItem.Station;
                            branchId = await ChartRepo.GetBranchIdAsync(cutS.BranchTitle, chartId);
                            prevStationId = await ChartRepo.GetStationIdAsync(cutS.Title, branchId);
                            MCMC.Station? ws = await ChartRepo.GetStationWeakByIdAsync(prevStationId);

                            if (ws != null)
                            {
                                MCUD.StationDTO station = new(
                                    ws.Title, cutS.BranchTitle, ws.Occupancy,
                                    DomainDtoConverter.Convert(ws.Type),
                                    ws.OpenTime, ws.CloseTime
                                    );
                                MCUD.RouteStationItemDTO stationItem = new(station);
                                path.Add(stationItem);
                            }

                            break;
                        }
                    case MCUD.RouteConnectionItemDTO routeConnectionItem:
                        {
                            switch (routeConnectionItem.Connection)
                            {
                                case MCUD.RailwayConnectionDTO railwayConnection:
                                    {
                                        MCUD.RailwayDTO cutR = railwayConnection.Railway;
                                        int nextStationId = await ChartRepo.GetStationIdAsync(cutR.NextStationTitle, branchId);
                                        int railwayId = await ChartRepo.GetRailwayIdAsync(prevStationId, nextStationId);
                                        MCMC.Railway? wr = await ChartRepo.GetRailwayByIdAsync(railwayId);

                                        if (wr != null)
                                        {
                                            MCUD.RailwayDTO railway = new(
                                                cutR.BranchTitle, cutR.PrevStationTitle,
                                                cutR.NextStationTitle, wr.Duration);
                                            MCUD.RailwayConnectionDTO railwayItem = new(railway);
                                            MCUD.RouteConnectionItemDTO routeCon = new(railwayItem);
                                            path.Add(routeCon);
                                        }

                                        break;
                                    }
                                case MCUD.TransitionConnectionDTO transitionConnection:
                                    {
                                        MCUD.TransitionDTO cutT = transitionConnection.Transition;
                                        branchId = await ChartRepo.GetBranchIdAsync(cutT.ToBranchTitle, chartId);
                                        int nextStationId = await ChartRepo.GetStationIdAsync(cutT.ToStationTitle, branchId);
                                        int transitionId = await ChartRepo.GetTransitionIdAsync(prevStationId, nextStationId);
                                        MCMC.Transition? wt = await ChartRepo.GetTransitionByIdAsync(transitionId);

                                        if (wt != null)
                                        {
                                            MCUD.TransitionDTO transition = new(
                                                wt.Occupancy, DomainDtoConverter.Convert(wt.Type),
                                                wt.Duration, wt.OpenTime, wt.CloseTime,
                                                cutT.FromStationTitle, cutT.FromBranchTitle,
                                                cutT.ToStationTitle, cutT.ToBranchTitle
                                            );
                                            MCUD.TransitionConnectionDTO transitionItem = new(transition);
                                            MCUD.RouteConnectionItemDTO routeCon = new(transitionItem);
                                            path.Add(routeCon);
                                        }

                                        break;
                                    }
                                default:
                                    throw new NotSupportedException($"Тип связи маршрута не поддерживается: {routeConnectionItem.GetType()}");
                            }

                            break;
                        }
                }
            }

        MCMC.Chart? chart = await ChartRepo.GetChartWeakByIdAsync(chartId);

        return chart != null && route != null ? new MCUD.RouteDTO(
            route.Title, chart.City, chart.Title, path, route.Duration
        ) : null;
    }
}