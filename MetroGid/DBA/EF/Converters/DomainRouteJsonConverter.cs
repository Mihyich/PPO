using System.ComponentModel;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using MetroGid.Controllers.Utility.DTO.Concrete;
using MetroGid.Core.Converters;
using MCMT = MetroGid.Core.Models.Types;
using MCMC = MetroGid.Core.Models.Concrete;
using TCC = MetroGid.Core.Converters;
using System.Text.Json.Serialization.Metadata;
using Microsoft.Extensions.Options;

namespace MetroGid.DBA.EF.Converters;

public static class DomainRouteJsonConverter
{
    public static string Convert(MCMC.Route route)
    {
        List<RouteItemJsonDTO> routeItems = [];

        foreach (var item in route.Path)
        {
            switch (item)
            {
                case MCMC.RouteStationItem rsi:
                    {
                        MCMC.Station mcmcS = rsi.Station;
                        StationRouteItemJsonDTO sri = new(
                            mcmcS.Title,
                            mcmcS.Branch?.Title ?? "",
                            mcmcS.Occupancy,
                            mcmcS.Type.ToString(),
                            mcmcS.OpenTime,
                            mcmcS.CloseTime
                        );
                        routeItems.Add(sri);
                        break;
                    }
                case MCMC.RouteConnectionItem rci:
                    {
                        switch (rci.Connection)
                        {
                            case MCMC.RailwayConnection rc:
                                {
                                    MCMC.Railway mcmcR = rc.Railway;
                                    RailwayRouteItemJsonDTO rri = new(
                                        mcmcR.Prev?.Branch?.Title ?? "",
                                        mcmcR.Prev?.Title ?? "",
                                        mcmcR.Next?.Title ?? "",
                                        mcmcR.Duration
                                    );
                                    routeItems.Add(rri);
                                    break;
                                }
                            case MCMC.TransitionConnection tc:
                                {
                                    MCMC.Transition mcmcT = tc.Transition;
                                    TransitionRouteItemJsonDTO tri = new(
                                        mcmcT.From?.Branch?.Title ?? "",
                                        mcmcT.From?.Title ?? "",
                                        mcmcT.To?.Branch?.Title ?? "",
                                        mcmcT.To?.Title ?? "",
                                        mcmcT.Occupancy,
                                        mcmcT.Type.ToString(),
                                        mcmcT.Duration,
                                        mcmcT.OpenTime,
                                        mcmcT.CloseTime
                                    );
                                    routeItems.Add(tri);
                                    break;
                                }
                        }

                        break;
                    }
            }
        }

        RouteJsonDTO routeJsonDTO = new(
            route.Chart?.City ?? "",
            route.Chart?.Title ?? "",
            route.Title,
            route.Duration,
            routeItems
        );

        JsonSerializerOptions JsonOptions = CreateJsonOptions();

        return JsonSerializer.Serialize(routeJsonDTO, JsonOptions);
    }

    public static MCMC.Route Convert(string jsonRoute)
    {
        JsonSerializerOptions JsonOptions = CreateJsonOptions();

        RouteJsonDTO routeDto = JsonSerializer.Deserialize<RouteJsonDTO>(jsonRoute, JsonOptions) ??
            throw new ArgumentException("Не удалось десериализовать маршрут.");

        List<MCMC.RouteItem> path = [];

        foreach (var item in routeDto.RouteItems)
        {
            switch (item)
            {
                case StationRouteItemJsonDTO sri:
                    {
                        MCMC.Station station = new(
                            sri.Title,
                            sri.Occupancy,
                            AccessTypeConverter.FromString(sri.Access),
                            sri.OpenTime,
                            sri.CloseTime
                        );
                        MCMC.RouteItem routeItem = new MCMC.RouteStationItem(station);
                        path.Add(routeItem);
                        break;
                    }

                case RailwayRouteItemJsonDTO rri:
                    {
                        MCMC.Railway railway = new(
                            rri.Duration
                        );
                        MCMC.StationConnection stationConnection = new MCMC.RailwayConnection(railway);
                        MCMC.RouteItem routeItem = new MCMC.RouteConnectionItem(stationConnection);
                        path.Add(routeItem);
                        break;
                    }

                case TransitionRouteItemJsonDTO tri:
                    {
                        MCMC.Transition transition = new(
                            tri.Occupancy,
                            AccessTypeConverter.FromString(tri.Access),
                            tri.Duration,
                            tri.OpenTime,
                            tri.CloseTime
                        );
                        MCMC.StationConnection stationConnection = new MCMC.TransitionConnection(transition);
                        MCMC.RouteItem routeItem = new MCMC.RouteConnectionItem(stationConnection);
                        path.Add(routeItem);
                        break;
                    }

                default:
                    throw new NotSupportedException($"Тип маршрута не поддерживается: {item.GetType()}");
            }
        }

        MCMC.Chart mcmcC = new(routeDto.ChartTitle, routeDto.City, "");
        StationRouteItemJsonDTO firstSRI = (StationRouteItemJsonDTO)routeDto.RouteItems[0];
        MCMC.Branch curMcmcB = new(firstSRI.BranchTitle, 0, MCMT.AccessType.ACCESSIBLE);

        curMcmcB.Chart = mcmcC;

        for (int i = 0; i < path.Count(); ++i)
        {
            RouteItemJsonDTO ri = routeDto.RouteItems[i];
            RouteItemJsonDTO? prevRi = i - 1 >= 0 ? routeDto.RouteItems[i - 1] : null;
            RouteItemJsonDTO? nextRi = i + 1 < routeDto.RouteItems.Count() ? routeDto.RouteItems[i + 1] : null;

            StationRouteItemJsonDTO? prevSri = prevRi != null && (prevRi is StationRouteItemJsonDTO) ? (StationRouteItemJsonDTO)prevRi : null;
            StationRouteItemJsonDTO? nextSri = nextRi != null && (nextRi is StationRouteItemJsonDTO) ? (StationRouteItemJsonDTO)nextRi : null;

            RailwayRouteItemJsonDTO? prevRri = prevRi != null && (prevRi is RailwayRouteItemJsonDTO) ? (RailwayRouteItemJsonDTO)prevRi : null;
            RailwayRouteItemJsonDTO? nextRri = nextRi != null && (nextRi is RailwayRouteItemJsonDTO) ? (RailwayRouteItemJsonDTO)nextRi : null;

            TransitionRouteItemJsonDTO? prevTri = prevRi != null && (prevRi is TransitionRouteItemJsonDTO) ? (TransitionRouteItemJsonDTO)prevRi : null;
            TransitionRouteItemJsonDTO? nextTri = nextRi != null && (nextRi is TransitionRouteItemJsonDTO) ? (TransitionRouteItemJsonDTO)nextRi : null;

            switch (path[i])
            {
                case MCMC.RouteStationItem rsi:
                    {
                        MCMC.Station mcmcS = rsi.Station;

                        if (prevRri != null)
                            mcmcS.Prev = ((MCMC.RailwayConnection)((MCMC.RouteConnectionItem)path[i - 1]).Connection).Railway;

                        if (nextRri != null)
                            mcmcS.Next = ((MCMC.RailwayConnection)((MCMC.RouteConnectionItem)path[i + 1]).Connection).Railway;

                        mcmcS.Branch = curMcmcB;

                        break;
                    }
                case MCMC.RouteConnectionItem rci:
                    {
                        switch (rci.Connection)
                        {
                            case MCMC.RailwayConnection rc:
                                {
                                    MCMC.Railway mcmcR = rc.Railway;

                                    if (prevSri != null)
                                        mcmcR.Prev = ((MCMC.RouteStationItem)path[i - 1]).Station;

                                    if (nextSri != null)
                                        mcmcR.Next = ((MCMC.RouteStationItem)path[i + 1]).Station;

                                    break;
                                }
                            case MCMC.TransitionConnection tc:
                                {
                                    MCMC.Transition mcmcT = tc.Transition;

                                    if (prevSri != null)
                                        mcmcT.From = ((MCMC.RouteStationItem)path[i - 1]).Station;

                                    if (nextSri != null)
                                    {
                                        mcmcT.To = ((MCMC.RouteStationItem)path[i + 1]).Station;
                                        curMcmcB = new(nextSri.BranchTitle, 0, MCMT.AccessType.ACCESSIBLE);
                                        curMcmcB.Chart = mcmcC;
                                    }

                                    break;
                                }
                            default:
                                throw new NotSupportedException($"Тип связи маршрута не поддерживается: {rci.GetType()}");
                        }

                        break;
                    }
                default:
                    throw new NotSupportedException($"Тип маршрута не поддерживается: {path[i].GetType()}");
            }
        }

        MCMC.Route mcmcRoute = new(
            routeDto.Title,
            path,
            routeDto.Duration
        );

        mcmcRoute.Chart = mcmcC;

        return mcmcRoute;
    }

    private static JsonSerializerOptions CreateJsonOptions()
    {
        var options = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };

        var resolver = new DefaultJsonTypeInfoResolver();

        resolver.Modifiers.Add(context =>
        {
            if (context.Type == typeof(RouteItemJsonDTO))
            {
                context.PolymorphismOptions = new JsonPolymorphismOptions
                {
                    TypeDiscriminatorPropertyName = "$type",
                    DerivedTypes =
                    {
                        new JsonDerivedType(typeof(StationRouteItemJsonDTO), "station"),
                        new JsonDerivedType(typeof(RailwayRouteItemJsonDTO), "railway"),
                        new JsonDerivedType(typeof(TransitionRouteItemJsonDTO), "transition")
                    }
                };
            }
        });

        options.TypeInfoResolver = resolver;
        return options;
    }
    
    private record RouteJsonDTO(
        string City,
        string ChartTitle,
        string Title,
        TimeSpan Duration,
        List<RouteItemJsonDTO> RouteItems
    );

    private abstract record RouteItemJsonDTO { };

    private record StationRouteItemJsonDTO(
        string Title,
        string BranchTitle,
        int Occupancy,
        string Access,
        TimeOnly OpenTime,
        TimeOnly CloseTime
    ) : RouteItemJsonDTO;

    private record RailwayRouteItemJsonDTO(
        string BranchTitle,
        string FromStationTitle,
        string ToStationTitle,
        TimeSpan Duration
    ) : RouteItemJsonDTO;

    private record TransitionRouteItemJsonDTO(
        string FromBranchTitle,
        string FromStationTitle,
        string ToBranchTitle,
        string ToStationTitle,
        int Occupancy,
        string Access,
        TimeSpan Duration,
        TimeOnly OpenTime,
        TimeOnly CloseTime
    ) : RouteItemJsonDTO;
}