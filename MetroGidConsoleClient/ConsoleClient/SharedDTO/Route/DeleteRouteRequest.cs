namespace ConsoleClient.SharedDTO.Route;

public record DeleteRouteRequest(
    string CityTitle,
    string ChartTitle,
    string RouteTitle
);