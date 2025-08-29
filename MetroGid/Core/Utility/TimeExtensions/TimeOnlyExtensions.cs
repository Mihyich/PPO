namespace MetroGid.Core.Utility.TimeExtensions;

public static class TimeOnlyExtensions
{
    public static TimeOnly Add(this TimeOnly time, TimeSpan span)
    {
        TimeSpan total = time.ToTimeSpan() + span;
        long ticks = total.Ticks % TimeSpan.FromDays(1).Ticks;

        if (ticks < 0)
            ticks += TimeSpan.FromDays(1).Ticks;
            
        return TimeOnly.FromTimeSpan(TimeSpan.FromTicks(ticks));
    }
}