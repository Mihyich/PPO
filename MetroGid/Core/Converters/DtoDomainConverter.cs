using MetroGid.Controllers.DTO;
using MetroGid.Core.Models.Concrete;
using MetroGid.Core.Models.Types;

namespace MetroGid.Core.Converters;

public static class DtoDomainConverter
{
    public static AccessType Convert(AccessTypeDTO type) =>
        type switch
        {
            AccessTypeDTO.ACCESSIBLE => AccessType.ACCESSIBLE,
            AccessTypeDTO.INACCESSIBLE => AccessType.INACCESSIBLE,
            _ => AccessType.INACCESSIBLE
        };

    public static RoleType Convert(RoleTypeDTO type) =>
        type switch
        {
            RoleTypeDTO.UNSIGNED => RoleType.UNSIGNED,
            RoleTypeDTO.SIGNED => RoleType.SIGNED,
            RoleTypeDTO.DUTY => RoleType.DUTY,
            _ => RoleType.UNSIGNED
        };

    public static Chart Convert(ChartDTO chart) =>
        new(chart.Title, chart.City, chart.SvgInst);

    public static Branch Convert(BranchDTO branch) =>
        new(branch.Title, branch.Color, Convert(branch.Type));

    public static Station Convert(StationDTO station) =>
        new(station.Title, station.Occupancy, Convert(station.Type),
            station.OpenTime, station.CloseTime);

    public static Railway Convert(RailwayDTO railway) =>
        new(railway.Duration);

    public static Transition Convert(TransitionDTO transition) =>
        new(transition.Occupancy, Convert(transition.Type), transition.Duration,
            transition.OpenTime, transition.CloseTime);

    public static Client Convert(ClientDTO client) =>
        new(client.Login, client.Password, client.Mail, Convert(client.Role));

    public static Route Convert(RouteDTO route)
    {
        List<RouteItem> ConPath = [];

        foreach (var item in route.Path)
        {
            if (item is RouteStationItemDTO { Station: var station })
            {
                ConPath.Add(new RouteStationItem(Convert(station)));
            }
            else if (item is RouteConnectionItemDTO { Connection: var connection })
            {
                if (connection is RailwayConnectionDTO { Railway: var railway })
                {
                    ConPath.Add(new RouteConnectionItem(new RailwayConnection(Convert(railway))));
                }
                else if (connection is TransitionConnectionDTO { Transition: var transition })
                {
                    ConPath.Add(new RouteConnectionItem(new TransitionConnection(Convert(transition))));
                }
            }
        }

        return new()
        {
            Title = route.Title,
            Path = ConPath,
            Duration = route.Duration
        };
    }
}