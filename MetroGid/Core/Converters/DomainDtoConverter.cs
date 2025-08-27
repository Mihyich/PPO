using MCUD = MetroGid.Controllers.Utility.DTO.Concrete;
using MCMC = MetroGid.Core.Models.Concrete;
using MCMT = MetroGid.Core.Models.Types;

namespace MetroGid.Core.Converters;

public static class DomainDtoConverter
{
    public static MCUD.AccessTypeDTO Convert(MCMT.AccessType type) =>
        type switch
        {
            MCMT.AccessType.ACCESSIBLE => MCUD.AccessTypeDTO.ACCESSIBLE,
            MCMT.AccessType.INACCESSIBLE => MCUD.AccessTypeDTO.INACCESSIBLE,
            _ => MCUD.AccessTypeDTO.INACCESSIBLE
        };

    public static MCUD.RoleTypeDTO Convert(MCMT.RoleType type) =>
        type switch
        {
            MCMT.RoleType.UNSIGNED => MCUD.RoleTypeDTO.UNSIGNED,
            MCMT.RoleType.SIGNED => MCUD.RoleTypeDTO.SIGNED,
            MCMT.RoleType.DUTY => MCUD.RoleTypeDTO.DUTY,
            _ => MCUD.RoleTypeDTO.UNSIGNED
        };

    public static MCUD.ChartDTO Convert(MCMC.Chart chart) =>
        new(chart.City, chart.Title, chart.SvgInst);

    public static MCUD.BranchDTO Convert(MCMC.Branch branch) =>
        new(branch.Title, branch.Color, Convert(branch.Type));

    public static MCUD.StationDTO Convert(MCMC.Station station) =>
        new(station.Title, station.Branch?.Title ?? string.Empty,
            station.Occupancy, Convert(station.Type),
            station.OpenTime, station.CloseTime);

    public static MCUD.RailwayDTO Convert(MCMC.Railway railway) =>
        new(railway.Prev?.Branch?.Title ?? railway.Next?.Branch?.Title ?? string.Empty,
            railway.Prev?.Title ?? string.Empty,
            railway.Next?.Title ?? string.Empty,
            railway.Duration);

    public static MCUD.TransitionDTO Convert(MCMC.Transition transition) =>
        new(transition.Occupancy, Convert(transition.Type),
            transition.Duration, transition.OpenTime, transition.CloseTime,
            transition.From?.Title ?? string.Empty,
            transition.From?.Branch?.Title ?? string.Empty,
            transition.To?.Title ?? string.Empty,
            transition.To?.Branch?.Title ?? string.Empty);

    public static MCUD.ClientDTO Convert(MCMC.Client client) =>
        new(client.Login, client.Password, client.Mail, Convert(client.Role));

    public static MCUD.RouteDTO Convert(MCMC.Route route)
    {
        List<MCUD.RouteItemDTO> CntPath = [];

        foreach (var item in route.Path)
        {
            if (item is MCMC.RouteStationItem { Station: var station })
                CntPath.Add(new MCUD.RouteStationItemDTO(Convert(station)));
            else if (item is MCMC.RouteConnectionItem { Connection: var connection })
            {
                if (connection is MCMC.RailwayConnection { Railway: var railway })
                    CntPath.Add(new MCUD.RouteConnectionItemDTO(new MCUD.RailwayConnectionDTO(Convert(railway))));
                else if (connection is MCMC.TransitionConnection { Transition: var transition })
                    CntPath.Add(new MCUD.RouteConnectionItemDTO(new MCUD.TransitionConnectionDTO(Convert(transition))));
            }
        }

        return new(route.Title, route.Chart?.City ?? string.Empty, route.Chart?.Title ?? string.Empty, CntPath, route.Duration);
    }
}