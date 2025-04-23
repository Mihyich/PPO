using MetroGid.Core.Models;

namespace MetroGid.Core.Utilities.Strategies
{
    public class StrategySearchRouteDijkstra : StrategySearchRouteBase
    {
        public override Route? Search(List<Branch> branches, Station src, Station dst)
        {
            Dictionary<Station, List<Route>> Adj = GenAdj(branches);
            Dictionary<Station, Route> dist = GenDist(branches); // Изначально Route.Duration = inf
            HashSet<Station> visited = [];
            PriorityQueue<Route, TimeSpan> pq = new();
            TimeSpan time;
            Station? lstation;

            Route route = new();
            route.Add(src);
            route.UpdateDuration();

            dist[src] = route;
            pq.Enqueue(route, route.Duration);

            while (pq.Count > 0)
            {
                route = pq.Dequeue();
                lstation = route.GetLastStation();
                time = route.Duration;

                if (lstation == null || time > dist[lstation].Duration)
                    continue;

                foreach (Route r in Adj[lstation])
                {
                    Station? neighbor = r.GetLastStation();
                    TimeSpan newTime = dist[lstation].Duration + r.Duration;
                    Route newRoute;

                    if (neighbor != null && !visited.Contains(neighbor) && dist[neighbor].Duration > newTime)
                    {
                        newRoute = route.Clone();
                        newRoute.PopBack();
                        newRoute.Merge(r);
                        newRoute.UpdateDuration();

                        dist[neighbor] = newRoute;
                        pq.Enqueue(newRoute, newRoute.Duration);
                    }
                }

                visited.Add(lstation);
            }

            return dist[dst];
        }

        private static Dictionary<Station, List<Route>> GenAdj(List<Branch> branches)
        {
            Dictionary<Station, List<Route>> Adj = [];

            foreach (var branch in branches)
            {
                foreach (var station in branch.Stations)
                {
                    Route route;
                    List<Route> routes = [];

                    Station? neighbor;
                    Railway? railPrev = station.Prev;
                    Railway? railNext = station.Next;

                    if (railPrev != null && (neighbor = railPrev.Prev) != null)
                    {
                        route = new();
                        route.Add(station);
                        route.Add(railPrev);
                        route.Add(neighbor);
                        route.UpdateDuration();
                        routes.Add(route);
                    }

                    if (railNext != null && (neighbor = railNext.Next) != null)
                    {
                        route = new();
                        route.Add(station);
                        route.Add(railNext);
                        route.Add(neighbor);
                        route.UpdateDuration();
                        routes.Add(route);
                    }

                    foreach (var transition in station.Transitions)
                    {
                        if ((neighbor = transition.ToFrom(station)) != null)
                        {
                            route = new();
                            route.Add(station);
                            route.Add(transition);
                            route.Add(neighbor);
                            route.UpdateDuration();
                            routes.Add(route);
                        }
                    }

                    Adj[station] = routes;
                }
            }

            return Adj;
        }

        private static Dictionary<Station, Route> GenDist(List<Branch> branches)
        {
            Dictionary<Station, Route> dist = [];

            foreach (var branch in branches)
                foreach (var station in branch.Stations)
                    dist[station] = new Route() { Duration = TimeSpan.MaxValue };

            return dist;
        }
    }
}