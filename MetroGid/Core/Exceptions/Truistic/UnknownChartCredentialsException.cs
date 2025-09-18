namespace MetroGid.Core.Exceptions.Truistic;

public class UnknownChartCredentialsException : Exception
{
    public string ChartTitle { get; }
    public string CityTitle { get; }

    public UnknownChartCredentialsException(string chartTitle, string cityTitle)
        : base($"Схема'{chartTitle}' в городе '{cityTitle}' не найдена")
    {
        ChartTitle = chartTitle;
        CityTitle = cityTitle;
    }
}