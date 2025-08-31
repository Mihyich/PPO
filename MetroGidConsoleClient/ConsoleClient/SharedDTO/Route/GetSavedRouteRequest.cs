namespace ConsoleClient.SharedDTO.Route;

public record GetSavedRouteRequest(
    string CityTitle,
    string ChartTitle,
    string RouteTitle
);