namespace MetroGid.Core.Utility.Validators.Predicators;

public static class ColorP
{
    public static readonly int MaxValue = 16777215;

    public static bool IsOutOfRange(int color) => color < 0 || color > MaxValue;
}