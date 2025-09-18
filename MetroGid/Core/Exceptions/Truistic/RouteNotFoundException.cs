namespace MetroGid.Core.Exceptions.Truistic;

public class RouteNotFoundException : Exception
{
    public string ChartTitle { get; }
    public string CityTitle { get; }
    public string BranchTitleSrc { get; }
    public string StationTitleSrc { get; }
    public string BranchTitleDst { get; }
    public string StationTitleDst { get; }
    public TimeOnly StartTime { get; }

    public RouteNotFoundException(
        string chartTitle, string cityTitle,
        string branchTitleSrc, string stationTitleSrc,
        string branchTitleDst, string stationTitleDst,
        TimeOnly startTime
    ) : base($"Не удалось найти маршрут в городе '{cityTitle}' в схеме '{chartTitle}' со станции '{stationTitleSrc}' ветки '{branchTitleSrc}' на станцию '{stationTitleDst}' ветки '{branchTitleDst}' в {startTime}")
    {
        ChartTitle = chartTitle;
        CityTitle = cityTitle;
        BranchTitleSrc = branchTitleSrc;
        StationTitleSrc = stationTitleSrc;
        BranchTitleDst = branchTitleDst;
        StationTitleDst = stationTitleDst;
        StartTime = startTime;
    }
}