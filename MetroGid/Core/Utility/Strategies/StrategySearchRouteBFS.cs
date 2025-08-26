using MCMC = MetroGid.Core.Models.Concrete;
using MetroGid.Core.Utility.TimeMeter.Concrete;
using MetroGid.Core.Utility.TimeMeter.Super;

namespace MetroGid.Core.Utility.Strategies;

public class StrategySearchRouteBFS(TimeSuper? timerSuper = null) : StrategySearchRouteBase(timerSuper ?? new TimeFast())
{
    public override MCMC.Route? Search(MCMC.Chart chart, MCMC.Station src, MCMC.Station dst, TimeOnly timeStart)
    {
        Queue<MCMC.Route> queue = new();
        HashSet<MCMC.Station> visited = [];
        MCMC.Route initialRoute = new(string.Empty, [], TimeSpan.Zero, ts);

        if ((!src.Branch?.IsAccessible() ?? true) || !src.IsAccessible() || !src.IsOpenAt(timeStart) ||
            (!dst.Branch?.IsAccessible() ?? true) || !dst.IsAccessible())
            return null;

        initialRoute.Add(src);
        queue.Enqueue(initialRoute);

        while (queue.Count > 0)
        {
            MCMC.Route curRoute = queue.Dequeue();
            MCMC.Station? lastStation = curRoute.GetLastStation();

            if (lastStation != null)
            {
                if (lastStation == dst)
                    return curRoute;

                SearchRailwayNeighbors(lastStation, curRoute, queue, visited);
                SearchTransitionNeighbors(lastStation, curRoute, queue, visited, timeStart);
            }
        }

        return null;
    }

    private static void SearchRailwayNeighbors(MCMC.Station curStation, MCMC.Route curRoute, Queue<MCMC.Route> queue, HashSet<MCMC.Station> visited)
    {
        MCMC.Railway? railwayNeigbor = curStation.Next;
        MCMC.Station? stationNeighbor;

        if (railwayNeigbor != null)
        {
            stationNeighbor = railwayNeigbor.Next;
            UpdateProcess(curRoute, railwayNeigbor, stationNeighbor, queue, visited);
        }

        railwayNeigbor = curStation.Prev;

        if (railwayNeigbor != null)
        {
            stationNeighbor = railwayNeigbor.Prev;
            UpdateProcess(curRoute, railwayNeigbor, stationNeighbor, queue, visited);
        }
    }

    private void SearchTransitionNeighbors(MCMC.Station curStation, MCMC.Route curRoute, Queue<MCMC.Route> queue, HashSet<MCMC.Station> visited, TimeOnly timeStart)
    {
        TimeOnly curTime = TimeOnly.FromTimeSpan(TimeSpan.FromTicks(curRoute.Duration.Ticks) + TimeSpan.FromTicks(timeStart.Ticks));
        MCMC.Station? stationNeighbor;

        foreach (var transitionNeighbor in curStation.Transitions)
            if (transitionNeighbor.IsAccessible() && transitionNeighbor.IsOpenAt(curTime) && (stationNeighbor = transitionNeighbor.ToFrom(curStation)) != null)
                UpdateProcess(curRoute, transitionNeighbor, stationNeighbor, queue, visited,
                    TimeOnly.FromTimeSpan(TimeSpan.FromTicks(curTime.Ticks) + ts.Measure(curStation, transitionNeighbor, stationNeighbor)));
    }

    private static void UpdateProcess(MCMC.Route curRoute, MCMC.Railway railwayNeigbor, MCMC.Station? stationNeighbor, Queue<MCMC.Route> queue, HashSet<MCMC.Station> visited)
    {
        if (stationNeighbor != null && !visited.Contains(stationNeighbor))
        {
            MCMC.Route newRoute = curRoute.SemiShallowClone().Append(railwayNeigbor).Append(stationNeighbor);
            queue.Enqueue(newRoute);
            visited.Add(stationNeighbor);
        }
    }

    private static void UpdateProcess(MCMC.Route curRoute, MCMC.Transition transitionNeigbor, MCMC.Station? stationNeighbor, Queue<MCMC.Route> queue, HashSet<MCMC.Station> visited, TimeOnly curTime)
    {
        if (stationNeighbor != null && stationNeighbor.IsAccessible() && stationNeighbor.IsOpenAt(curTime) && (stationNeighbor?.Branch?.IsAccessible() ?? false) && !visited.Contains(stationNeighbor))
        {
            MCMC.Route newRoute = curRoute.SemiShallowClone().Append(transitionNeigbor).Append(stationNeighbor);
            queue.Enqueue(newRoute);
            visited.Add(stationNeighbor);
        }
    }
}