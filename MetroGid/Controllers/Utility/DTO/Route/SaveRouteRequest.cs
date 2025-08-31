namespace MetroGid.Controllers.Utility.DTO.Route;

public record SaveRouteRequest(
    string CityTitle,
    string ChartTitle,
    string routeJson
);