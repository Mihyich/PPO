namespace MetroGid.Core.Utilities.Validators.Predicators;

public static class LoginP
{
    public static readonly int MinLength = 1;
    public static readonly int MaxLength = 255;

    public static bool IsOutOfRange(string l) =>
        l.Length < MinLength || l.Length > MaxLength;
}