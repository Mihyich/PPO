namespace ConsoleClient.SharedDTO.Chart;

public record PatchTransitionRequest(
    string CityTitle,
    string ChartTitle,
    string FromBranchTitle,
    string FromStationTitle,
    string ToBranchTitle,
    string ToStationTitle,

    int NewOccupancy,
    string NewAccess,
    string NewDuration,
    string NewOpenTime,
    string NewCloseTime
);