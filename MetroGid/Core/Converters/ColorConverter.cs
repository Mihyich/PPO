namespace MetroGid.Core.Converters;

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