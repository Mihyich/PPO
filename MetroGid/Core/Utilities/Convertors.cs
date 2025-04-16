using MetroGid.Core.Models;

namespace MetroGid.Core.Utilities
{
    public static class ColorConverter
    {
        public static int FromHex(string hexColor)
        {
            if (string.IsNullOrWhiteSpace(hexColor))
                return 0;

            hexColor = hexColor.TrimStart('#');

            return hexColor.Length switch
            {
                3 => Convert.ToInt32($"{hexColor[0]}{hexColor[0]}{hexColor[1]}{hexColor[1]}{hexColor[2]}{hexColor[2]}", 16),
                6 => Convert.ToInt32(hexColor, 16),
                _ => 0
            };
        }
    }

    public static class AccessTypeConverter
    {
        public static AccessType FromString(string accessType)
        {
            return accessType?.ToUpperInvariant() switch
            {
                "ACCESSIBLE" => AccessType.ACCESSIBLE,
                "INACCESSIBLE" => AccessType.INACCESSIBLE,
                _ => AccessType.INACCESSIBLE
            };
        }
    }

    public static class TimeConverter
    {
        public static TimeOnly FromString(string timeString)
        {
            if (TimeOnly.TryParse(timeString, out var result))
                return result;

            return new TimeOnly(0, 0);
        }
    }
}