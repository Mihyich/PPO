namespace MetroGid.Controllers.Utility.DTO.Route;

public record GetSavedRouteRequest(
    string CityTitle,
    string ChartTitle,
    string RouteTitle
);