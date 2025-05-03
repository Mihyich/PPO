using System.Text.RegularExpressions;

namespace MetroGid.Core.Utilities.Validators.Predicators;

public static class MailP
{
    public static bool IsValid(string m)
    {
        string pattern = "[.\\-_a-z0-9]+@([a-z0-9][\\-a-z0-9]+\\.)+[a-z]{2,6}";
        Match isMatch = Regex.Match(m, pattern, RegexOptions.IgnoreCase);
        return isMatch.Success;
    }
}