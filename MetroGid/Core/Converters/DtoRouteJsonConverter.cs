using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using MCUD = MetroGid.Controllers.Utility.DTO.Concrete;

namespace MetroGid.Core.Converters;

public static class DtoRouteJsonConverter
{
    public static string Convert(MCUD.RouteDTO route)
    {
        List<RouteItemJsonDTO> routeItems = [];

        foreach (var item in route.Path)
        {
            switch (item)
            {
                case MCUD.RouteStationItemDTO routeStationItem:
                {
                    MCUD.StationDTO s = routeStationItem.Station;
                    StationRouteItemDTO sri = new(s.Title, s.BranchTitle);
                    routeItems.Add(sri);
                    break;
                }
                case MCUD.RouteConnectionItemDTO routeConnectionItem:
                {
                    switch (routeConnectionItem.Connection)
                    {
                        case MCUD.RailwayConnectionDTO railwayConnection:
                        {
                            MCUD.RailwayDTO r = railwayConnection.Railway;
                            RailwayRouteItemDTO rri = new(r.PrevStationTitle, r.NextStationTitle);
                            routeItems.Add(rri);
                            break;
                        }
                        case MCUD.TransitionConnectionDTO transitionConnection:
                        {
                            MCUD.TransitionDTO t = transitionConnection.Transition;
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

    public static MCUD.RouteDTO Convert(string json)
    {
        RouteJsonDTO routeJsonDto = JsonSerializer.Deserialize<RouteJsonDTO>(json, JsonOptions)
            ?? throw new JsonException("Не удалось десериализовать JSON в RouteJsonDTO.");

        MCUD.StationDTO? psd = null;
        List<MCUD.RouteItemDTO> path = [];

        foreach (var item in routeJsonDto.RouteItems)
        {
            switch (item)
            {
                case StationRouteItemDTO sri:
                {
                    MCUD.StationDTO station = new(sri.Title, sri.BranchTitle, 0, MCUD.AccessTypeDTO.INACCESSIBLE, TimeOnly.MinValue, TimeOnly.MinValue);
                    MCUD.RouteStationItemDTO stationItem = new(station);
                    path.Add(stationItem);
                    psd = station;
                    break;
                }
                case RailwayRouteItemDTO rri:
                {
                    MCUD.RailwayDTO railway = new(psd?.BranchTitle ?? throw new JsonException($"Некорректный маршрут"), rri.FromStationTitle, rri.ToStationTitle, TimeSpan.MinValue);
                    MCUD.RailwayConnectionDTO connection = new(railway);
                    MCUD.RouteConnectionItemDTO connectionItem = new(connection);
                    path.Add(connectionItem);
                    break;
                }
                case TransitionRouteItemDTO tri:
                {
                    MCUD.TransitionDTO transition = new(
                        0, MCUD.AccessTypeDTO.INACCESSIBLE, TimeSpan.MinValue,
                        TimeOnly.MinValue, TimeOnly.MinValue,
                        tri.FromStationTitle, tri.FromBranchTitle,
                        tri.ToStationTitle, tri.ToBranchTitle
                    );
                    MCUD.TransitionConnectionDTO connection = new(transition);
                    MCUD.RouteConnectionItemDTO connectionItem = new(connection);
                    path.Add(connectionItem);
                    break;
                }
                default:
                    throw new JsonException($"Неизвестный тип элемента маршрута: {item.GetType()}");
            }
        }

        return new MCUD.RouteDTO(routeJsonDto.Title, "", "", path, routeJsonDto.Duration);
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