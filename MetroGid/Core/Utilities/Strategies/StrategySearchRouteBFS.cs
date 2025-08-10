using MetroGid.Core.Models.Concrete;
using MetroGid.Core.Utilities.TimeMeter.Concrete;
using MetroGid.Core.Utilities.TimeMeter.Super;

namespace MetroGid.Core.Utilities.Strategies;

public class StrategySearchRouteBFS(TimeSuper? timerSuper = null) : StrategySearchRouteBase(timerSuper ?? new TimeFast())
{
    public override Route Search(Chart chart, Station src, Station dst, TimeOnly timeStart)
    {
        Queue<Route> queue = new();
        HashSet<Station> visited = [];
        Route initialRoute = new(string.Empty, [], TimeSpan.Zero, ts);

        if ((!src.Branch?.IsAccessible() ?? true) || !src.IsAccessible() || !src.IsOpenAt(timeStart) ||
            (!dst.Branch?.IsAccessible() ?? true) || !dst.IsAccessible())
            return initialRoute;

        initialRoute.Add(src);
        queue.Enqueue(initialRoute);

        while (queue.Count > 0)
        {
            Route curRoute = queue.Dequeue();
            Station? lastStation = curRoute.GetLastStation();

            if (lastStation != null)
            {
                if (lastStation == dst)
                    return curRoute;

                SearchRailwayNeighbors(lastStation, curRoute, queue, visited);
                SearchTransitionNeighbors(lastStation, curRoute, queue, visited, timeStart);
            }
        }

        return initialRoute;
    }

    private static void SearchRailwayNeighbors(Station curStation, Route curRoute, Queue<Route> queue, HashSet<Station> visited)
    {
        Railway? railwayNeigbor = curStation.Next;
        Station? stationNeighbor;

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

    private void SearchTransitionNeighbors(Station curStation, Route curRoute, Queue<Route> queue, HashSet<Station> visited, TimeOnly timeStart)
    {
        TimeOnly curTime = TimeOnly.FromTimeSpan(TimeSpan.FromTicks(curRoute.Duration.Ticks) + TimeSpan.FromTicks(timeStart.Ticks));
        Station? stationNeighbor;

        foreach (var transitionNeighbor in curStation.Transitions)
            if (transitionNeighbor.IsAccessible() && transitionNeighbor.IsOpenAt(curTime) && (stationNeighbor = transitionNeighbor.ToFrom(curStation)) != null)
                UpdateProcess(curRoute, transitionNeighbor, stationNeighbor, queue, visited,
                    TimeOnly.FromTimeSpan(TimeSpan.FromTicks(curTime.Ticks) + ts.Measure(curStation, transitionNeighbor, stationNeighbor)));
    }

    private static void UpdateProcess(Route curRoute, Railway railwayNeigbor, Station? stationNeighbor, Queue<Route> queue, HashSet<Station> visited)
    {
        if (stationNeighbor != null && !visited.Contains(stationNeighbor))
        {
            Route newRoute = curRoute.SemiShallowClone().Append(railwayNeigbor).Append(stationNeighbor);
            queue.Enqueue(newRoute);
            visited.Add(stationNeighbor);
        }
    }

    private static void UpdateProcess(Route curRoute, Transition transitionNeigbor, Station? stationNeighbor, Queue<Route> queue, HashSet<Station> visited, TimeOnly curTime)
    {
        if (stationNeighbor != null && stationNeighbor.IsAccessible() && stationNeighbor.IsOpenAt(curTime) && (stationNeighbor?.Branch?.IsAccessible() ?? false) && !visited.Contains(stationNeighbor))
        {
            Route newRoute = curRoute.SemiShallowClone().Append(transitionNeigbor).Append(stationNeighbor);
            queue.Enqueue(newRoute);
            visited.Add(stationNeighbor);
        }
    }
}