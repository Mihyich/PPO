using System.Security.Claims;
using MetroGid.Controllers.Attributes;
using MetroGid.Controllers.Utility.DTO.Concrete;
using MetroGid.Controllers.Utility.DTO.Route;
using MetroGid.Controllers.Utility.Interfaces;
using MetroGid.Core.Converters;
using MCMA = MetroGid.Core.Models.Advanced;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MCMC = MetroGid.Core.Models.Concrete;

namespace MetroGid.Controllers.Concrete;

[ApiController]
[Route("api/routes")]
public class RouteController(
    IRouteService routeService
) : ControllerBase
{
    private readonly IRouteService _routeService = routeService;

    private int GetClientAndChartIdAsync()
    {
        string? clientIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!int.TryParse(clientIdClaim, out int clientId))
            return 0; // Нужно исключение

        return clientId;
    }

    [AllowAnonymous]
    [HttpPost("search")]
    public async Task<RouteDTO> SearchRoute([FromBody] SearchRouteRequest dto)
    {
        MCMC.Route? route = await _routeService.SearchRouteAsync(
            dto.CityTitle, dto.ChartTitle,
            dto.FromBranchTitle, dto.FromStationTitle,
            dto.ToBranchTitle, dto.ToStationTitle,
            TimeConverter.FromString(dto.CurTime)
        );

        return DomainDtoConverter.Convert(route);
    }

    [RequireAnyRole(RoleTypeDTO.SIGNED, RoleTypeDTO.DUTY)]
    [HttpPost("save")]
    public async Task<int> SaveRoute([FromBody] RouteDTO dto)
    {
        int clientId = GetClientAndChartIdAsync();
        MCMC.Route route = DtoDomainConverter.Convert(dto);
        MCMA.IdRow routeIdRow = await _routeService.SaveRouteAsync(clientId, route);
        return routeIdRow.id;
    }

    [RequireAnyRole(RoleTypeDTO.SIGNED, RoleTypeDTO.DUTY)]
    [HttpPost("get/titles")]
    public async Task<IActionResult> GetRouteTitles([FromBody] GetRouteCredentialsRequest dto)
    {
        int clientId = GetClientAndChartIdAsync();

        return Ok(
            await _routeService.GetSavedChartRoutesTitles(
                clientId,
                dto.CityTitle,
                dto.ChartTitle
            )
        );
    }

    [RequireAnyRole(RoleTypeDTO.SIGNED, RoleTypeDTO.DUTY)]
    [HttpPost("get/saved")]
    public async Task<RouteDTO> GetSavedRoute([FromBody] GetSavedRouteRequest dto)
    {
        int clientId = GetClientAndChartIdAsync();

        MCMC.Route? route = await _routeService.GetSavedChart(
            clientId,
            dto.CityTitle,
            dto.ChartTitle,
            dto.RouteTitle
        );

        return DomainDtoConverter.Convert(route);
    }

    [RequireAnyRole(RoleTypeDTO.SIGNED, RoleTypeDTO.DUTY)]
    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteRoute([FromBody] DeleteRouteRequest dto)
    {
        int clientId = GetClientAndChartIdAsync();

        return Ok(
            await _routeService.DeleteAsync(
                clientId,
                dto.RouteTitle
            )
        );
    }
}