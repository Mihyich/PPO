using MCMC = MetroGid.Core.Models.Concrete;
using MCMT = MetroGid.Core.Models.Types;

using MDEMT = MetroGid.DBA.EF.Models.Tables;
using MDEME = MetroGid.DBA.EF.Models.UserDefinedTypes;

using System.Text.Json.Serialization;
using MetroGid.Core.Models.Concrete;
using System.Text.Json;

namespace MetroGid.DBA.EF.Converters;

public static class DomainModelConverter
{
    public static MDEME.AccessType Convert(MCMT.AccessType type) =>
        type switch
        {
            MCMT.AccessType.ACCESSIBLE => MDEME.AccessType.ACCESSIBLE,
            MCMT.AccessType.INACCESSIBLE => MDEME.AccessType.INACCESSIBLE,
            _ => MDEME.AccessType.INACCESSIBLE
        };

    public static MDEME.NexusType Convert(MCMT.WayType type) =>
        type switch
        {
            MCMT.WayType.STATION => MDEME.NexusType.STATION,
            MCMT.WayType.RAILWAY => MDEME.NexusType.RAILWAY,
            MCMT.WayType.TRANSITION => MDEME.NexusType.TRANSITION,
            _ => MDEME.NexusType.STATION
        };

    public static MDEME.RoleType Convert(MCMT.RoleType type) =>
        type switch
        {
            MCMT.RoleType.UNSIGNED => MDEME.RoleType.UNSIGNED,
            MCMT.RoleType.SIGNED => MDEME.RoleType.SIGNED,
            MCMT.RoleType.DUTY => MDEME.RoleType.DUTY,
            _ => MDEME.RoleType.UNSIGNED
        };

    public static MDEMT.Chart Convert(MCMC.Chart chart) =>
        new()
        {
            City = chart.City,
            Title = chart.Title,
            SvgContent = chart.SvgInst
        };

    public static MDEMT.Branch Convert(MCMC.Branch branch) =>
        new()
        {
            Title = branch.Title,
            Color = branch.Color,
            Access = Convert(branch.Type)
        };

    public static MDEMT.Station Convert(MCMC.Station station) =>
        new()
        {
            Title = station.Title,
            Occupancy = (short)station.Occupancy,
            Access = Convert(station.Type),
            OpenTime = station.OpenTime,
            CloseTime = station.CloseTime
        };

    public static MDEMT.Railway Convert(MCMC.Railway railway) =>
        new()
        {
            // FromId,
            // ToId,
            Duration = railway.Duration
        };

    public static MDEMT.Transition Convert(MCMC.Transition transition) =>
        new()
        {
            Occupancy = (short)transition.Occupancy,
            Access = Convert(transition.Type),
            Duration = transition.Duration,
            OpenTime = transition.OpenTime,
            CloseTime = transition.CloseTime
        };

    public static MDEMT.Client Convert(MCMC.Client client) =>
        new()
        {
            ClientLogin = client.Login,
            ClientPassword = client.Password,
            Mail = client.Mail,
            Privilege = Convert(client.Role)  
        };

    public static string Convert(MCMC.Route route)
    {
        List<RouteItemJsonDTO> routeItems = [];
        Station? ps = null;

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
                    Station ts = t.ToFrom(
                        ps ?? throw new Exception("Некорректная маршрут")
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
        return JsonSerializer.Serialize(routeJsonDTO);
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
    private abstract class RouteItemJsonDTO
    {
        public abstract RouteItemType Type { get; }
    };

    private class StationRouteItemDTO(string title, string branchTitle) : RouteItemJsonDTO
    {
        public override RouteItemType Type => RouteItemType.STATION;
        public string Title { get; } = title;
        public string BranchTitle { get; } = branchTitle;
    };

    private class RailwayRouteItemDTO(
        string fromStationTitle, string toStationTitle
    ) : RouteItemJsonDTO
    {
        public override RouteItemType Type => RouteItemType.RAILWAY;

        public string FromStationTitle { get; } = fromStationTitle;
        public string ToStationTitle { get; } = toStationTitle;
    };

    private class TransitionRouteItemDTO(
        string fromBranchTitle, string fromStationTitle,
        string toBranchTitle, string toStationTitle
    ) : RouteItemJsonDTO
    {
        public override RouteItemType Type => RouteItemType.TRANSITION;
        public string FromBranchTitle { get; } = fromBranchTitle;
        public string FromStationTitle { get; } = fromStationTitle;
        public string ToBranchTitle { get; } = toBranchTitle;
        public string ToStationTitle { get; } = toStationTitle;
    };
}