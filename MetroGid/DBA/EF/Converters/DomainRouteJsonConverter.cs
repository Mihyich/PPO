using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using MCMC = MetroGid.Core.Models.Concrete;

namespace MetroGid.DBA.EF.Converters;

public static class DomainRouteJsonConverter
{
    public static string Convert(MCMC.Route route)
    {
        List<RouteItemJsonDTO> routeItems = [];
        MCMC.Station? ps = null;

        foreach (var item in route.Path)
        {
            // Станция
            if (item is MCMC.RouteStationItem { Station: MCMC.Station s })
            {
                ps = s;
                StationRouteItemDTO sri = new(s.Title, s.Branch?.Title ?? string.Empty);
                routeItems.Add(sri);
            }
            else if (item is MCMC.RouteConnectionItem { Connection: MCMC.StationConnection connection })
            {
                // Переезд
                if (connection is MCMC.RailwayConnection { Railway: MCMC.Railway r })
                {
                    RailwayRouteItemDTO rri = new(r.Prev?.Title ?? string.Empty, r.Next?.Title ?? string.Empty);
                    routeItems.Add(rri);
                }
                // Переход
                else if (connection is MCMC.TransitionConnection { Transition: MCMC.Transition t })
                {
                    MCMC.Station ts = t.ToFrom(
                        ps ?? throw new Exception("Некорректный маршрут")
                    ) ?? throw new Exception("Некорректная схема");

                    TransitionRouteItemDTO tri = new(
                        ps.Branch?.Title ?? string.Empty, ps.Title,
                        ts.Branch?.Title ?? string.Empty, ts.Title
                    );

                    routeItems.Add(tri);
                }
            }
        }

        RouteJsonDTO routeJsonDTO = new(route.Title, route.Duration, routeItems);
        return JsonSerializer.Serialize(routeJsonDTO, JsonOptions);
    }

    public static MCMC.Route Convert(string jsonRoute)
    {
        RouteJsonDTO routeDto = JsonSerializer.Deserialize<RouteJsonDTO>(jsonRoute, JsonOptions) ??
            throw new ArgumentException("Не удалось десериализовать маршрут.");

        List<MCMC.RouteItem> path = [];

        foreach (var item in routeDto.RouteItems)
        {
            switch (item)
            {
                case StationRouteItemDTO stationRouteItem:
                {
                    MCMC.Station station = new(stationRouteItem.Title, 0, 0, TimeOnly.MinValue, TimeOnly.MinValue);
                    MCMC.RouteItem routeItem = new MCMC.RouteStationItem(station);
                    path.Add(routeItem);
                    break;
                }

                case RailwayRouteItemDTO railwayRouteItemDTO:
                {
                    MCMC.Railway railway = new(TimeSpan.MinValue);
                    MCMC.StationConnection stationConnection = new MCMC.RailwayConnection(railway);
                    MCMC.RouteItem routeItem = new MCMC.RouteConnectionItem(stationConnection);
                    path.Add(routeItem);
                    break;
                }

                case TransitionRouteItemDTO transitionRouteItemDTO:
                {
                    MCMC.Transition transition = new(0, 0, TimeSpan.MinValue, TimeOnly.MinValue,TimeOnly.MinValue);
                    MCMC.StationConnection stationConnection = new MCMC.TransitionConnection(transition);
                    MCMC.RouteItem routeItem = new MCMC.RouteConnectionItem(stationConnection);
                    path.Add(routeItem);
                    break;
                }

                default:
                    throw new NotSupportedException($"Тип маршрута не поддерживается: {item.GetType()}");
            }
        }

        return new MCMC.Route(routeDto.Title, path, routeDto.Duration);
    }

    private class RouteJsonDTO(string title, TimeSpan duration, List<RouteItemJsonDTO> routeItems)
    {
        public string Title { get; } = title;
        public TimeSpan Duration { get; } = duration;
        public List<RouteItemJsonDTO> RouteItems { get; } = routeItems;
    };

    private enum RouteItemType
    {
        STATION = 0,
        RAILWAY,
        TRANSITION
    };

    [JsonDerivedType(typeof(StationRouteItemDTO), typeDiscriminator: "station")]
    [JsonDerivedType(typeof(RailwayRouteItemDTO), typeDiscriminator: "railway")]
    [JsonDerivedType(typeof(TransitionRouteItemDTO), typeDiscriminator: "transition")]
    private abstract class RouteItemJsonDTO{};

    private class StationRouteItemDTO(string title, string branchTitle) : RouteItemJsonDTO
    {
        public string Title { get; } = title;
        public string BranchTitle { get; } = branchTitle;
    };

    private class RailwayRouteItemDTO(
        string fromStationTitle, string toStationTitle
    ) : RouteItemJsonDTO
    {

        public string FromStationTitle { get; } = fromStationTitle;
        public string ToStationTitle { get; } = toStationTitle;
    };

    private class TransitionRouteItemDTO(
        string fromBranchTitle, string fromStationTitle,
        string toBranchTitle, string toStationTitle
    ) : RouteItemJsonDTO
    {
        public string FromBranchTitle { get; } = fromBranchTitle;
        public string FromStationTitle { get; } = fromStationTitle;
        public string ToBranchTitle { get; } = toBranchTitle;
        public string ToStationTitle { get; } = toStationTitle;
    };

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, // Кириллица
        WriteIndented = true // Форматирование с отступами
    };
}