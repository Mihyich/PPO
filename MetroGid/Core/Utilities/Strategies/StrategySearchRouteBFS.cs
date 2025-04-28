using MetroGid.Core.Models.Concrete;

namespace MetroGid.Core.Utilities.Strategies
{
    public class StrategySearchRouteBFS : StrategySearchRouteBase
    {
        public override Route? Search(List<Branch> branches, Station src, Station dst, TimeOnly timeStart)
        {
            Queue<Route> queue = new();
            HashSet<Station> visited = [];
            Route initialRoute = new();

            if ((!src.Branch?.IsAccessible() ?? false) || !src.IsAccessible() || !src.IsOpenAt(timeStart))
                return null;

            initialRoute.Add(src);
            queue.Enqueue(initialRoute);

            while (queue.Count > 0)
            {
                Route curRoute = queue.Dequeue();
                Station? lastStation = curRoute.GetLastStation();

                curRoute.UpdateDuration();

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

        private static void SearchTransitionNeighbors(Station curStation, Route curRoute, Queue<Route> queue, HashSet<Station> visited, TimeOnly timeStart)
        {
            TimeOnly curTime = TimeOnly.FromTimeSpan(TimeSpan.FromTicks(curRoute.Duration.Ticks) + TimeSpan.FromTicks(timeStart.Ticks));
            Station? stationNeighbor;

            foreach (var transitionNeighbor in curStation.Transitions)
                if (transitionNeighbor.IsAccessible() && transitionNeighbor.IsOpenAt(curTime) && (stationNeighbor = transitionNeighbor.ToFrom(curStation)) != null)
                    UpdateProcess(curRoute, transitionNeighbor, stationNeighbor, queue, visited,
                        TimeOnly.FromTimeSpan(TimeSpan.FromTicks(curTime.Ticks) + TimeMeas.Measure(curStation, transitionNeighbor, stationNeighbor)));
        }

        private static void UpdateProcess(Route curRoute, Railway railwayNeigbor, Station? stationNeighbor, Queue<Route> queue, HashSet<Station> visited)
        {
            if (stationNeighbor != null && !visited.Contains(stationNeighbor))
            {
                Route newRoute = curRoute.Clone();
                newRoute.Add(railwayNeigbor);
                newRoute.Add(stationNeighbor);
                queue.Enqueue(newRoute);
                visited.Add(stationNeighbor);
            }
        }

        private static void UpdateProcess(Route curRoute, Transition transitionNeigbor, Station? stationNeighbor, Queue<Route> queue, HashSet<Station> visited, TimeOnly curTime)
        {
            if (stationNeighbor != null && stationNeighbor.IsAccessible() && stationNeighbor.IsOpenAt(curTime) && (stationNeighbor?.Branch?.IsAccessible() ?? false) && !visited.Contains(stationNeighbor))
            {
                Route newRoute = curRoute.Clone();
                newRoute.Add(transitionNeigbor);
                newRoute.Add(stationNeighbor);
                queue.Enqueue(newRoute);
                visited.Add(stationNeighbor);
            }
        }

        // private void UpdateProcess<TConnection>(
        //     Route curRoute, TConnection connection,
        //     Station? stationNeighbor, Queue<Route> queue, HashSet<Station> visited
        // ) where TConnection : GraphConnection
        // {
        //     if (stationNeighbor == null || visited.Contains(stationNeighbor)) 
        //         return;

        //     Route newRoute = curRoute.Clone();
        //     newRoute.Add(connection);
        //     newRoute.Add(stationNeighbor);
        //     queue.Enqueue(newRoute);
        //     visited.Add(stationNeighbor);
        // }
    }
}