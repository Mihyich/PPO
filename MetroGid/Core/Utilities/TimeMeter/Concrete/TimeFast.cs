using MetroGid.Core.Models.Concrete;
using MetroGid.Core.Utilities.TimeMeter.Super;

namespace MetroGid.Core.Utilities.TimeMeter.Concrete;

public class TimeFast : TimeSuper
{
    public override TimeSpan Measure(RouteItem from, RouteItem to)
    {
        if (from is RouteStationItem { Station: var stationFrom })
            return Measure(stationFrom, to);
        else if (from is RouteConnectionItem { Connection: var conFrom })
            return Measure(conFrom, to);
        
        return TimeSpan.Zero;
    }

    private TimeSpan Measure(Station s, RouteItem ri)
    {
        if (ri is RouteStationItem {Station: _ })
            return TimeSpan.Zero;
        
        if (ri is RouteConnectionItem { Connection: var conTo })
        {
            if (conTo is RailwayConnection { Railway: var railwayTo })
                return Measure(s, railwayTo);
            else if (conTo is TransitionConnection { Transition: var transitionTo })
                return Measure(s, transitionTo);
        }
        
        return TimeSpan.Zero;
    }

    private TimeSpan Measure(StationConnection sc, RouteItem ri)
    {
        if (sc is RailwayConnection { Railway: var railwayFrom })
            return Measure(railwayFrom, ri);
        else if (sc is TransitionConnection { Transition: var transitionFrom })
            return Measure(transitionFrom, ri);
        
        return TimeSpan.Zero;
    }

    private TimeSpan Measure(Railway r, RouteItem ri) =>
        (ri is RouteStationItem { Station: var stationTo }) ? Measure(r, stationTo) : TimeSpan.Zero;

    private TimeSpan Measure(Transition t, RouteItem ri) =>
        (ri is RouteStationItem {Station: var stationTo}) ? Measure(t, stationTo) : TimeSpan.Zero;


    public override TimeSpan Measure(Station s, Railway r) =>
        WaitOnStation(s) + r.Duration.ToTimeSpan() / 2;

    public override TimeSpan Measure(Railway r, Station s) => 
        r.Duration.ToTimeSpan() / 2;

    public override TimeSpan Measure(Station s1, Railway r, Station s2) =>
        Measure(s1, r) + Measure(r, s2);
    


    public override TimeSpan Measure(Station s, Transition t) =>
        MoveOnStation(s) + MoveOnTransition(t) / 2;

    public override TimeSpan Measure(Transition t, Station s) =>
        Measure(s, t);

    public override TimeSpan Measure(Station s1, Transition t, Station s2) =>
        Measure(s1, t) + Measure(t, s2);
    


    private static TimeSpan WaitOnStation(Station s) =>
        TimeSpan.FromSeconds(6 * Math.Max(s.Occupancy, 1));

    private static TimeSpan MoveOnStation(Station s) =>
        TimeSpan.FromSeconds(10 * Math.Max(s.Occupancy, 1));

    private static TimeSpan MoveOnTransition(Transition t) =>
        TimeSpan.FromSeconds(t.Duration.ToTimeSpan().TotalSeconds * Math.Max(t.Occupancy / 2, 1));
}