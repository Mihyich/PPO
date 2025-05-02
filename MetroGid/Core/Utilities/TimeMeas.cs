using MetroGid.Core.Models.Concrete;

namespace MetroGid.Core.Utilities;

public static class TimeMeas
{
    public static TimeSpan Measure(Station s1, Railway r) =>
        WaitOnStation(s1) + r.Duration.ToTimeSpan();

    public static TimeSpan Measure(Station s1, Transition t, Station s2) =>
        MoveOnStation(s1) + MoveOnTransition(t) + MoveOnStation(s2);

    private static TimeSpan WaitOnStation(Station s) =>
        TimeSpan.FromSeconds(6 * Math.Max(s.Occupancy, 1));

    private static TimeSpan MoveOnStation(Station s) =>
        TimeSpan.FromSeconds(10 * Math.Max(s.Occupancy, 1));

    private static TimeSpan MoveOnTransition(Transition t) =>
        TimeSpan.FromSeconds(t.Duration.ToTimeSpan().TotalSeconds * Math.Max(t.Occupancy / 2, 1));
}