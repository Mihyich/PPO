using MetroGid.Core.Utilities.TimeMeter.Concrete;
using MetroGid.Core.Utilities.TimeMeter.Super;
using MetroGid.Core.Utilities.Validators.Interfaces;

namespace MetroGid.Core.Models.Concrete;

public abstract record StationConnection;
public record RailwayConnection(Railway Railway) : StationConnection;
public record TransitionConnection(Transition Transition) : StationConnection;

public abstract record RouteItem;
public record RouteStationItem(Station Station) : RouteItem;
public record RouteConnectionItem(StationConnection Connection) : RouteItem;

public class Route(string title, List<RouteItem> path, TimeSpan duration, TimeSuper? timeSuper = null) : IDomainValidatorAccepter
{
    public string Title { get; set; } = title;
    public List<RouteItem> Path { get; private set; } = path;
    public TimeSpan Duration { get; private set; } = duration;
    public Chart? Chart;

    private readonly TimeSuper ts = timeSuper ?? new TimeFast();

    public Route(Route other) : this(other.Title, new List<RouteItem>(other.Path), other.Duration)
    {
        Chart = other.Chart;
    }

    public void Add(RouteItem item)
    {
        Path.Add(item);
        UpdateDurationAfterAddRouteItem();
    }

    public void Add(StationConnection connection)
    {
        Path.Add(new RouteConnectionItem(connection));
        UpdateDurationAfterAddRouteItem();
    }

    public void Add(Station station) => Add(new RouteStationItem(station));
    public void Add(Railway railway) => Add(new RailwayConnection(railway));
    public void Add(Transition transition) => Add(new TransitionConnection(transition));

    public void AddAsOrphan(Route other)
    {
        for (int i = 1; i < other.Path.Count; ++i)
            Add(other.Path[i]);
    }

    public Route Append(RouteItem item)
    {
        Add(item);
        return this;
    }

    public Route Append(StationConnection connection)
    {
        Add(new RouteConnectionItem(connection));
        return this;
    }

    public Route Append(Station station) => Append(new RouteStationItem(station));
    public Route Append(Railway railway) => Append(new RailwayConnection(railway));
    public Route Append(Transition transition) => Append(new TransitionConnection(transition));

    public Route AppendAsOrphan(Route other)
    {
        AddAsOrphan(other);
        return this;
    }

    public void RemoveLast()
    {
        UpdateDurationBeforeRemoveLastRouteItem();

        if (Path.Count > 0)
            Path.RemoveAt(Path.Count - 1);
    }

    public Route PopBack()
    {
        RemoveLast();
        return this;
    }

    public Station? GetLastStation() => (Path.LastOrDefault(p => p is RouteStationItem) as RouteStationItem)?.Station;

    public Railway? GetLastRailway() =>
        ((Path.LastOrDefault(p => p is RouteConnectionItem { Connection: RailwayConnection }) as RouteConnectionItem)?.Connection as RailwayConnection)?.Railway;

    public Transition? GetLastTransition() =>
        ((Path.LastOrDefault(p => p is RouteConnectionItem { Connection: TransitionConnection }) as RouteConnectionItem)?.Connection as TransitionConnection)?.Transition;

    public Route SemiShallowClone() =>
        new (Title, new List<RouteItem>(Path), Duration)
        {
            Chart = this.Chart
        };

    public void SemiShallowCopy(Route other)
    {
        Title = other.Title;
        Path = new List<RouteItem>(other.Path);
        Duration = other.Duration;
        Chart = other.Chart;
    }

    public int GetStationCount() => Path.Count(p => p is RouteStationItem);

    public int GetRailwayCount() => Path.Count(p => p is RouteConnectionItem { Connection: RailwayConnection });

    public int GetTransitionCount() => Path.Count(p => p is RouteConnectionItem { Connection: TransitionConnection });

    public void UpdateDuration()
    {
        TimeSpan NewDuration = TimeSpan.Zero;

        for (int i = 2; i < Path.Count; ++i)
        {
            RouteItem from = Path[i - 1];
            RouteItem to = Path[i - 0];
            NewDuration += ts.Measure(from, to);
        }

        Duration = NewDuration;
    }

    private void UpdateDurationAfterAddRouteItem()
    {
        if (Path.Count < 2)
            return;

        int li = Path.Count - 1;
        RouteItem from = Path[li - 1];
        RouteItem to = Path[li];

        Duration += ts.Measure(from, to);
    }

    private void UpdateDurationBeforeRemoveLastRouteItem()
    {
        if (Path.Count < 2)
            return;

        int li = Path.Count - 1;
        RouteItem from = Path[li - 1];
        RouteItem to = Path[li];

        Duration -= ts.Measure(from, to);
    }

    public TimeSpan PredictDurationAfterAddAsOrphan(Route other) =>
        Duration + other.Duration;

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

    public void Validate(IDomainValidatorVisitor visitor) => visitor.Visit(this);
}