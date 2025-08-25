using System.Text.RegularExpressions;

namespace MetroGid.Core.Utility.Validators.Predicators;
public static class MailP
{
    public static readonly int MaxLength = 255;

    public static bool IsOutOfRange(string p) =>
        p.Length > MaxLength;

    public static bool IsValid(string m)
    {
        string pattern =
            @"^[a-z0-9!#$%&'*+/=?^_`{|}~-]+" +
            @"(\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*" +
            @"@" +
            @"([a-z0-9]([a-z0-9-]*[a-z0-9])?\.)+" +
            @"[a-z]{2,}$";

        Match isMatch = Regex.Match(m, pattern, RegexOptions.IgnoreCase);
        return isMatch.Success;
    }
}