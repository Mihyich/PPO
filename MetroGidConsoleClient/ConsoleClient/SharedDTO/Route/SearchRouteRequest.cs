namespace ConsoleClient.SharedDTO.Route;

public record SearchRouteRequest(
    string CityTitle,
    string ChartTitle,
    string FromBranchTitle,
    string FromStationTitle,
    string ToBranchTitle,
    string ToStationTitle,
    string CurTime
);