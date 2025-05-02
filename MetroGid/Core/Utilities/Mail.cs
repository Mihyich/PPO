using System.Text.RegularExpressions;

namespace MetroGid.Core.Utilities;

public static class Mail
{
    public static bool IsCorrect(string mail)
    {
        string pattern = "[.\\-_a-z0-9]+@([a-z0-9][\\-a-z0-9]+\\.)+[a-z]{2,6}";
        Match isMatch = Regex.Match(mail, pattern, RegexOptions.IgnoreCase);
        return isMatch.Success;
    }
}