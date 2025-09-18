namespace MetroGid.Core.Exceptions.Truistic;

public class UnknownStationCredentialsException : Exception
{
    public string StationTitle { get; }
    public int BranchId { get; }

    public UnknownStationCredentialsException(string stationTitle, int branchId)
        : base($"Станция '{stationTitle}' ветки с айди {branchId}")
    {
        StationTitle = stationTitle;
        BranchId = branchId;
    }
}