using System.Text.RegularExpressions;

namespace MetroGid.Core.Utility.Validators.Predicators;

public static class PasswordP
{
    public static readonly int MinLength = 6;
    public static readonly int MaxLength = 255;

    public static bool IsOutOfRange(string p) =>
        p.Length < MinLength || p.Length > MaxLength;

    public static bool IsValid(string p)
    {
        if (!Regex.IsMatch(p, @"[A-ZА-Я]"))
            return false;

        if (!Regex.IsMatch(p, @"[a-zа-я]"))
            return false;

        if (!Regex.IsMatch(p, @"\d"))
            return false;

        if (!Regex.IsMatch(p, @"^[A-ZА-Яa-zа-я0-9_!@#$%^&*()\-+=\[\]{}|:;,.?<>~`""]+$"))
            return false;

        return true;
    }
}