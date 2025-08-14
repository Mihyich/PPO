namespace MetroGid.Core.Utilities.Validators.Predicators;

public static class PasswordP
{
    public static readonly int MinLength = 6;
    public static readonly int MaxLength = 255;

    public static bool IsOutOfRange(string p) =>
        p.Length < MinLength || p.Length > MaxLength;

}