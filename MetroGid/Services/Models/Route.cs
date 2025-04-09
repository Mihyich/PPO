namespace MetroGid.Services.Models
{
    public abstract record GraphConnection;
    public record RailwayConnection(Railway Railway) : GraphConnection;
    public record TransitionConnection(Transition Transition) : GraphConnection;

    public abstract record RouteItem;
    public record RouteStationItem(Station Station) : RouteItem;
    public record RouteConnectionItem(GraphConnection Connection) : RouteItem;

    public class Route
    {
        public List<RouteItem> Path = [];

        public void Add(Station station) => Path.Add(new RouteStationItem(station));
        public void Add(GraphConnection connection) => Path.Add(new RouteConnectionItem(connection));
        public void Add(Railway railway) => Add(new RailwayConnection(railway));
        public void Add(Transition transition) => Add(new TransitionConnection(transition));

        public Station? GetLastStation()
        {
            return Path.LastOrDefault() switch
            {
                RouteStationItem item => item.Station,
                _ => null
            };
        }

        public Route Clone()
        {
            Route Route = new();

            foreach (var item in Path)
            {
                if (item is RouteStationItem { Station: var station })
                    Route.Add(station);
                else if (item is RouteConnectionItem { Connection: var connection })
                    Route.Add(connection);
            }

            return Route;
        }

        public bool IsValid()
        {
            for (int i = 0; i < Path.Count; ++i)
            {
                bool shouldBeStation = i % 2 == 0;
                bool isStation = Path[i] is RouteStationItem;
                
                if (shouldBeStation != isStation)
                    return false;
            }

            return true;
        }

        public void Output()
        {
            foreach (var item in Path)
            {
                if (item is RouteStationItem { Station: var station })
                    Console.WriteLine($"Станция: {station.Title}");
                else if (item is RouteConnectionItem { Connection: var connection })
                {
                    if (connection is RailwayConnection {Railway: var railway})
                        Console.WriteLine($"Переезд: {railway.Duration}");
                    else if (connection is TransitionConnection {Transition: var transition})
                        Console.WriteLine($"Переход: {transition.Duration}");
                }
            }
        }
    }
}