using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using MetroGid.Controllers.Utility.DTO;

namespace MetroGid.Core.Converters;

public static class DtoRouteJsonConverter
{
    public static string Convert(RouteDTO route)
    {
        List<RouteItemJsonDTO> routeItems = [];

        foreach (var item in route.Path)
        {
            switch (item)
            {
                case RouteStationItemDTO routeStationItem:
                {
                    StationDTO s = routeStationItem.Station;
                    StationRouteItemDTO sri = new(s.Title, s.BranchTitle);
                    routeItems.Add(sri);
                    break;
                }
                case RouteConnectionItemDTO routeConnectionItem:
                {
                    switch (routeConnectionItem.Connection)
                    {
                        case RailwayConnectionDTO railwayConnection:
                        {
                            RailwayDTO r = railwayConnection.Railway;
                            RailwayRouteItemDTO rri = new(r.PrevStationTitle, r.NextStationTitle);
                            routeItems.Add(rri);
                            break;
                        }
                        case TransitionConnectionDTO transitionConnection:
                        {
                            TransitionDTO t = transitionConnection.Transition;
                            TransitionRouteItemDTO tri = new(t.FromBranchTitle, t.FromStationTitle, t.ToBranchTitle, t.ToStationTitle);
                            routeItems.Add(tri);
                            break;
                        }
                        default:
                            throw new NotSupportedException($"Тип связи маршрута не поддерживается: {routeConnectionItem.GetType()}");
                    }
                    
                    break;
                }
                default:
                    throw new NotSupportedException($"Тип маршрута не поддерживается: {item.GetType()}");
            }
        }

        RouteJsonDTO routeJsonDTO = new(route.Title, route.Duration, routeItems);
        return JsonSerializer.Serialize(routeJsonDTO, JsonOptions);
    }

    private record RouteJsonDTO(
        string Title,
        TimeSpan Duration,
        List<RouteItemJsonDTO> RouteItems);

    private enum RouteItemType
    {
        STATION = 0,
        RAILWAY,
        TRANSITION
    };

    [JsonDerivedType(typeof(StationRouteItemDTO), typeDiscriminator: "station")]
    [JsonDerivedType(typeof(RailwayRouteItemDTO), typeDiscriminator: "railway")]
    [JsonDerivedType(typeof(TransitionRouteItemDTO), typeDiscriminator: "transition")]
    private abstract record RouteItemJsonDTO;

    private record StationRouteItemDTO(
        string Title,
        string BranchTitle
    ) : RouteItemJsonDTO;

    private record RailwayRouteItemDTO(
        string FromStationTitle,
        string ToStationTitle
    ) : RouteItemJsonDTO;

    private record TransitionRouteItemDTO(
        string FromBranchTitle,
        string FromStationTitle,
        string ToBranchTitle,
        string ToStationTitle
    ) : RouteItemJsonDTO;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, // Поддержка кириллицы
        WriteIndented = true // Красивое форматирование
    };
}