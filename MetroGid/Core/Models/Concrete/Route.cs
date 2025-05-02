using MetroGid.Core.Utilities;
using MetroGid.Core.Utilities.Validators.Interfaces;

namespace MetroGid.Core.Models.Concrete;

public abstract record StationConnection;
public record RailwayConnection(Railway Railway) : StationConnection;
public record TransitionConnection(Transition Transition) : StationConnection;

public abstract record RouteItem;
public record RouteStationItem(Station Station) : RouteItem;
public record RouteConnectionItem(StationConnection Connection) : RouteItem;

public class Route : IDomainValidatorAccepter
{
    public string Title = string.Empty;
    public List<RouteItem> Path = [];
    public TimeSpan Duration = TimeSpan.Zero;
    public Chart? Chart;

    public void UpdateDuration()
    {
        TimeSpan NewDuration = TimeSpan.Zero;

        if (Path.Count >= 3 && IsValid())
        {
            for (int i = 2; i < Path.Count; i += 2)
            {
                Station src = ((RouteStationItem)Path[i - 2]).Station;
                StationConnection connection = ((RouteConnectionItem)Path[i - 1]).Connection;
                Station dst = ((RouteStationItem)Path[i - 0]).Station;

                if (connection is RailwayConnection railcon)
                {
                    Railway railway = railcon.Railway;
                    NewDuration += TimeMeas.Measure(src, railway);
                }
                else if (connection is TransitionConnection trancon)
                {
                    Transition transition = trancon.Transition;
                    NewDuration += TimeMeas.Measure(src, transition, dst);
                }
            }
        }

        Duration = NewDuration;
    }

    public void Add(RouteItem item) => Path.Add(item);
    public void Add(Station station) => Path.Add(new RouteStationItem(station));
    public void Add(StationConnection connection) => Path.Add(new RouteConnectionItem(connection));
    public void Add(Railway railway) => Add(new RailwayConnection(railway));
    public void Add(Transition transition) => Add(new TransitionConnection(transition));

    public void PopBack()
    {
        if (Path.Count > 0)
            Path.RemoveAt(Path.Count - 1);
    }

    public void Merge(Route other) => Path.AddRange(other.Path);

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

        Route.Duration = Duration;

        return Route;
    }

    public bool IsValid()
    {
        bool shouldBeStation;
        bool isStation = false;

        for (int i = 0; i < Path.Count; ++i)
        {
            shouldBeStation = i % 2 == 0;
            isStation = Path[i] is RouteStationItem;

            if (shouldBeStation != isStation)
                return false;
        }

        return isStation;
    }

    public bool IsReferenceEquals(Route? route)
    {
        if (route == null)
            return false;

        if (ReferenceEquals(this, route))
            return true;

        if (!ReferenceEquals(Chart, route.Chart))
            return false;

        if (Duration != route.Duration)
            return false;

        if (Path.Count != route.Path.Count)
            return false;

        if (Title != route.Title)
            return false;

        for (int i = 0; i < Path.Count; ++i)
            if (!ReferenceEquals(Path[i], route.Path[i]))
                return false;

        return true;
    }

    public int GetStationCount() => IsValid() ? Path.Count / 2 + Path.Count % 2 : 0;

    public int GetTransitionCount()
    {
        int cnt = 0;

        if (IsValid())
            foreach (var item in Path)
                if (item is RouteConnectionItem { Connection: var connection } &&
                    connection is TransitionConnection { Transition: var transition })
                    ++cnt;

        return cnt;
    }

    public void Output()
    {
        if (!IsValid())
            return;

        int StationCnt = GetStationCount();
        int TransitionCnt = GetTransitionCount();

        Console.WriteLine($"Название: {Title}");
        Console.WriteLine($"Количество станций:   {StationCnt}");
        Console.WriteLine($"Количество пересадок: {TransitionCnt}");
        Console.WriteLine($"Время в пути: {Duration}");

        foreach (var item in Path)
        {
            if (item is RouteStationItem { Station: var station })
            {
                Console.WriteLine($"|---Станция: {station.Title}");
            }
            else if (item is RouteConnectionItem { Connection: var connection })
            {
                if (connection is RailwayConnection { Railway: var railway })
                    Console.WriteLine($"|===Переезд: {railway.Duration}");
                else if (connection is TransitionConnection { Transition: var transition })
                    Console.WriteLine($"|>>>Переход: {transition.Duration}");
            }
        }
    }

    public void Validate(IDomainValidatorVisitor visitor) => visitor.Visit(this);
}