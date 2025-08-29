namespace MetroGid.Core.Converters;

public static class TimeSpanConverter
{
    public static TimeSpan FromString(string timeString)
    {
        if (TimeSpan.TryParse(timeString, out var result))
            return result;

        return new TimeSpan(0, 0, 0);
    }
}