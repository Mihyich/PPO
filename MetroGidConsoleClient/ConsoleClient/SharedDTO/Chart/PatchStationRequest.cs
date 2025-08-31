namespace ConsoleClient.SharedDTO.Chart;

public record PatchStationRequest(
    string CityTitle,
    string ChartTitle,
    string BranchTitle,
    string StationTitle,

    string NewTitle,
    int NewOccupancy,
    string NewAccess,
    string NewOpenTime,
    string NewCloseTime
);