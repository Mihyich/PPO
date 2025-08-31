namespace ConsoleClient.SharedDTO.Chart;

public record PatchRailwayRequest(
    string CityTitle,
    string ChartTitle,

    string BranchTitle,
    string DutyStationTitle,
    string ToStationTitle,

    string NewDuration
);