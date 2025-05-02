namespace MetroGid.Core.Converters;

public static class TimeConverter
{
    public static TimeOnly FromString(string timeString)
    {
        if (TimeOnly.TryParse(timeString, out var result))
            return result;

        return new TimeOnly(0, 0);
    }
}