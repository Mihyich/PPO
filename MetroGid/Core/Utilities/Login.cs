namespace MetroGid.Core.Utilities;

public static class Login
{
    public static bool IsCorrect(string l) =>
        l.Length >= 4 && l.Length <= 255;
}