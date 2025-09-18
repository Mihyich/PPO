namespace MetroGid.Core.Exceptions.Truistic;

public class UnknownBranchCredentialsException : Exception
{
    public string ChartTitle { get; }
    public string CityTitle { get; }
    public string BranchTitle { get; }

    public UnknownBranchCredentialsException(string chartTitle, string cityTitle, string branchTitle)
        : base($"Ветка '{branchTitle}' в cхеме'{chartTitle}' в городе '{cityTitle}' не найдена")
    {
        ChartTitle = chartTitle;
        CityTitle = cityTitle;
        BranchTitle = branchTitle;
    }
}