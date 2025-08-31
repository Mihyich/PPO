namespace ConsoleClient.SharedDTO.Route;

public record SaveRouteRequest(
    string CityTitle,
    string ChartTitle,
    string routeJson
);