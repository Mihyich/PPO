namespace MetroGid.Core.Utility.Validators.Predicators;

public static class TitleP
{
    public static readonly int MaxLength = 255;
    public static bool IsEmpty(string title) => title.Length == 0;
    public static bool IsOutOfRange(string title) => title.Length > 255;
}