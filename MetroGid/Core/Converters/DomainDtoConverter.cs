using MetroGid.Controllers.Utility.DTO;
using MetroGid.Core.Models.Concrete;
using MetroGid.Core.Models.Types;

namespace MetroGid.Core.Converters;

public static class DomainDtoConverter
{
    public static AccessTypeDTO Convert(AccessType type) =>
        type switch
        {
            AccessType.ACCESSIBLE => AccessTypeDTO.ACCESSIBLE,
            AccessType.INACCESSIBLE => AccessTypeDTO.INACCESSIBLE,
            _ => AccessTypeDTO.INACCESSIBLE
        };

    public static RoleTypeDTO Convert(RoleType type) =>
        type switch
        {
            RoleType.UNSIGNED => RoleTypeDTO.UNSIGNED,
            RoleType.SIGNED => RoleTypeDTO.SIGNED,
            RoleType.DUTY => RoleTypeDTO.DUTY,
            _ => RoleTypeDTO.UNSIGNED
        };

    public static ChartDTO Convert(Chart chart) =>
        new(chart.City, chart.Title, chart.SvgInst);

    public static BranchDTO Convert(Branch branch) =>
        new(branch.Title, branch.Color, Convert(branch.Type));

    public static StationDTO Convert(Station station) =>
        new(station.Title, station.Branch?.Title ?? string.Empty,
            station.Occupancy, Convert(station.Type),
            station.OpenTime, station.CloseTime);

    public static RailwayDTO Convert(Railway railway) =>
        new(railway.Prev?.Branch?.Title ?? railway.Next?.Branch?.Title ?? string.Empty,
            railway.Prev?.Title ?? string.Empty,
            railway.Next?.Title ?? string.Empty,
            railway.Duration);

    public static TransitionDTO Convert(Transition transition) =>
        new(transition.Occupancy, Convert(transition.Type),
            transition.Duration, transition.OpenTime, transition.CloseTime,
            transition.From?.Title ?? string.Empty,
            transition.From?.Branch?.Title ?? string.Empty,
            transition.To?.Title ?? string.Empty,
            transition.To?.Branch?.Title ?? string.Empty);

    public static ClientDTO Convert(Client client) =>
        new(client.Login, client.Password, client.Mail, Convert(client.Role));

    public static RouteDTO Convert(Route route)
    {
        List<RouteItemDTO> CntPath = [];

        foreach (var item in route.Path)
        {
            if (item is RouteStationItem { Station: var station })
                CntPath.Add(new RouteStationItemDTO(Convert(station)));
            else if (item is RouteConnectionItem { Connection: var connection })
            {
                if (connection is RailwayConnection { Railway: var railway })
                    CntPath.Add(new RouteConnectionItemDTO(new RailwayConnectionDTO(Convert(railway))));
                else if (connection is TransitionConnection { Transition: var transition })
                    CntPath.Add(new RouteConnectionItemDTO(new TransitionConnectionDTO(Convert(transition))));
            }
        }

        return new(route.Title, route.Chart?.City ?? string.Empty, route.Chart?.Title ?? string.Empty, CntPath, route.Duration);
    }
}