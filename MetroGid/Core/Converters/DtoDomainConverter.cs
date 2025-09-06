using MCUD = MetroGid.Controllers.Utility.DTO.Concrete;
using MCMC = MetroGid.Core.Models.Concrete;
using MCMT = MetroGid.Core.Models.Types;
using MetroGid.Core.Models.Concrete;

namespace MetroGid.Core.Converters;

public static class DtoDomainConverter
{
    public static MCMT.AccessType Convert(MCUD.AccessTypeDTO type) =>
        type switch
        {
            MCUD.AccessTypeDTO.ACCESSIBLE => MCMT.AccessType.ACCESSIBLE,
            MCUD.AccessTypeDTO.INACCESSIBLE => MCMT.AccessType.INACCESSIBLE,
            _ => MCMT.AccessType.INACCESSIBLE
        };

    public static MCMT.RoleType Convert(MCUD.RoleTypeDTO type) =>
        type switch
        {
            MCUD.RoleTypeDTO.UNSIGNED => MCMT.RoleType.UNSIGNED,
            MCUD.RoleTypeDTO.SIGNED => MCMT.RoleType.SIGNED,
            MCUD.RoleTypeDTO.DUTY => MCMT.RoleType.DUTY,
            _ => MCMT.RoleType.UNSIGNED
        };

    public static MCMC.Chart Convert(MCUD.ChartDTO chart) =>
        new(chart.Title, chart.City, chart.SvgInst);

    public static MCMC.Branch Convert(MCUD.BranchDTO branch) =>
        new(branch.Title, branch.Color, Convert(branch.Type));

    public static MCMC.Station Convert(MCUD.StationDTO station) =>
        new(station.Title, station.Occupancy, Convert(station.Type),
            station.OpenTime, station.CloseTime);

    public static MCMC.Railway Convert(MCUD.RailwayDTO railway) =>
        new(railway.Duration);

    public static MCMC.Transition Convert(MCUD.TransitionDTO transition) =>
        new(transition.Occupancy, Convert(transition.Type), transition.Duration,
            transition.OpenTime, transition.CloseTime);

    public static MCMC.Client Convert(MCUD.ClientDTO client) =>
        new(client.Login, client.Password, client.Mail, Convert(client.Role));

    public static MCMC.Route Convert(MCUD.RouteDTO route)
    {
        List<MCMC.RouteItem> path = [];

        foreach (var item in route.Path)
        {
            switch (item)
            {
                case MCUD.RouteStationItemDTO routeStationItem:
                    {
                        MCUD.StationDTO mcudS = routeStationItem.Station;
                        MCMC.Station mcmcS = Convert(mcudS);
                        MCMC.RouteStationItem rsi = new(mcmcS);
                        path.Add(rsi);
                        break;
                    }
                case MCUD.RouteConnectionItemDTO routeConnectionItem:
                    {
                        switch (routeConnectionItem.Connection)
                        {
                            case MCUD.RailwayConnectionDTO railwayConnection:
                                {
                                    MCUD.RailwayDTO mcudR = railwayConnection.Railway;
                                    MCMC.Railway mcmcR = Convert(mcudR);
                                    MCMC.RailwayConnection rc = new(mcmcR);
                                    MCMC.RouteConnectionItem rci = new(rc);
                                    path.Add(rci);
                                    break;
                                }
                            case MCUD.TransitionConnectionDTO transitionConnection:
                                {
                                    MCUD.TransitionDTO mcudT = transitionConnection.Transition;
                                    MCMC.Transition mcmcT = Convert(mcudT);
                                    MCMC.TransitionConnection tc = new(mcmcT);
                                    MCMC.RouteConnectionItem rci = new(tc);
                                    path.Add(rci);
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

        MCMC.Chart mcmcC = new(route.ChartTitle, route.City, "");
        MCUD.StationDTO firstMcudS = ((MCUD.RouteStationItemDTO)route.Path[0]).Station;
        MCMC.Branch curMcmcB = new(firstMcudS.BranchTitle, 0, MCMT.AccessType.ACCESSIBLE);

        curMcmcB.Chart = mcmcC;

        for (int i = 0; i < path.Count(); ++i)
        {
            MCUD.RouteItemDTO ri = route.Path[i];

            MCUD.RouteItemDTO? prevMcudRi = i - 1 >= 0 ? route.Path[i - 1] : null;
            MCUD.RouteItemDTO? nextMcudRi = i + 1 < path.Count() ? route.Path[i + 1] : null;

            MCUD.RouteStationItemDTO? prevMcudRsi = prevMcudRi != null && (prevMcudRi is MCUD.RouteStationItemDTO) ? (MCUD.RouteStationItemDTO)prevMcudRi : null;
            MCUD.RouteStationItemDTO? nextMcudRsi = nextMcudRi != null && (nextMcudRi is MCUD.RouteStationItemDTO) ? (MCUD.RouteStationItemDTO)nextMcudRi : null;

            MCUD.StationDTO? prevMcudS = prevMcudRsi?.Station;
            MCUD.StationDTO? nextMcudS = nextMcudRsi?.Station;

            MCUD.RouteConnectionItemDTO? prevMcudCi = prevMcudRi != null && (prevMcudRi is MCUD.RouteConnectionItemDTO) ? (MCUD.RouteConnectionItemDTO)prevMcudRi : null;
            MCUD.RouteConnectionItemDTO? nextMcudCi = nextMcudRi != null && (nextMcudRi is MCUD.RouteConnectionItemDTO) ? (MCUD.RouteConnectionItemDTO)nextMcudRi : null;

            MCUD.RailwayConnectionDTO? prevMcudRc = prevMcudCi != null && (prevMcudCi.Connection is MCUD.RailwayConnectionDTO) ? (MCUD.RailwayConnectionDTO)prevMcudCi.Connection : null;
            MCUD.RailwayConnectionDTO? nextMcudRc = nextMcudCi != null && (nextMcudCi.Connection is MCUD.RailwayConnectionDTO) ? (MCUD.RailwayConnectionDTO)nextMcudCi.Connection : null;

            MCUD.RailwayDTO? prevMcudR = prevMcudRc?.Railway;
            MCUD.RailwayDTO? nextMcudR = nextMcudRc?.Railway;

            MCUD.TransitionConnectionDTO? prevMcudTc = prevMcudCi != null && (prevMcudCi.Connection is MCUD.TransitionConnectionDTO) ? (MCUD.TransitionConnectionDTO)prevMcudCi.Connection : null;
            MCUD.TransitionConnectionDTO? nextMcudTc = nextMcudCi != null && (nextMcudCi.Connection is MCUD.TransitionConnectionDTO) ? (MCUD.TransitionConnectionDTO)nextMcudCi.Connection : null;

            MCUD.TransitionDTO? prevMcudT = prevMcudTc?.Transition;
            MCUD.TransitionDTO? nextMcudT = nextMcudTc?.Transition;

            switch (path[i])
            {
                case MCMC.RouteStationItem rsi:
                    {
                        MCMC.Station mcmcS = rsi.Station;

                        if (prevMcudR != null)
                            mcmcS.Prev = ((MCMC.RailwayConnection)((MCMC.RouteConnectionItem)path[i - 1]).Connection).Railway;

                        if (nextMcudR != null)
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

                                    if (prevMcudS != null)
                                        mcmcR.Prev = ((MCMC.RouteStationItem)path[i - 1]).Station;

                                    if (nextMcudS != null)
                                        mcmcR.Next = ((MCMC.RouteStationItem)path[i + 1]).Station;

                                    break;
                                }
                            case MCMC.TransitionConnection tc:
                                {
                                    MCMC.Transition mcmcT = tc.Transition;

                                    if (prevMcudS != null)
                                        mcmcT.From = ((MCMC.RouteStationItem)path[i - 1]).Station;

                                    if (nextMcudS != null)
                                    {
                                        mcmcT.To = ((MCMC.RouteStationItem)path[i + 1]).Station;
                                        curMcmcB = new(nextMcudS.BranchTitle, 0, MCMT.AccessType.ACCESSIBLE);
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

        MCMC.Route mcmcRoute = new(route.Title, path, route.Duration);
        mcmcRoute.Chart = mcmcC;

        return mcmcRoute;
    }
}