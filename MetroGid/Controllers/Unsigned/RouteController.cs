using System.Security.Claims;
using MetroGid.Controllers.Attributes;
using MetroGid.Controllers.Utility.DTO.Concrete;
using MetroGid.Controllers.Utility.DTO.Route;
using MetroGid.Controllers.Utility.Interfaces;
using MetroGid.Core.Converters;
using MetroGid.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MetroGid.Controllers.Unsigned;

[ApiController]
[Route("api/route")]
public class RouteController(
    IRouteService routeService,
    IChartService chartService
) : ControllerBase
{
    private readonly IRouteService _routeService = routeService;
    private readonly IChartService _chartService = chartService;

    [AllowAnonymous]
    [HttpPost("search")]
    public async Task<IActionResult> SearchRoute([FromBody] SearchRouteRequest dto) =>
        Ok(
            await _routeService.SearchRouteAsync(
                dto.CityTitle, dto.ChartTitle,
                dto.FromBranchTitle, dto.FromStationTitle,
                dto.ToBranchTitle, dto.ToStationTitle,
                TimeConverter.FromString(dto.CurTime)
            )
        );

    [RequireAnyRole(RoleTypeDTO.SIGNED, RoleTypeDTO.DUTY)]
    [HttpPost("Save")]
    public async Task<IActionResult> SaveRoute([FromBody] RouteDTO dto)
    {
        string? clientIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(clientIdClaim, out int clientId))
            return StatusCode(
                StatusCodes.Status401Unauthorized,
                new
                {
                    Error = "InvalidCredentials",
                    Message = "Не удалось определить пользователя"
                }
            );

        int chartId = await _chartService.GetChartIdAsync(
            dto.City,
            dto.ChartTitle
        );

        return Ok(
            await _routeService.SaveRouteAsync(
                clientId,
                chartId,
                DtoRouteJsonConverter.Convert(dto)
            )
        );
    }

    [RequireAnyRole(RoleTypeDTO.SIGNED, RoleTypeDTO.DUTY)]
    [HttpPost("route_titles")]
    public async Task<IActionResult> GetRouteTitles([FromBody] GetRouteCredentialsRequest dto)
    {
        string? clientIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(clientIdClaim, out int clientId))
            return StatusCode(
                StatusCodes.Status401Unauthorized,
                new
                {
                    Error = "InvalidCredentials",
                    Message = "Не удалось определить пользователя"
                }
            );

        int chartId = await _chartService.GetChartIdAsync(
            dto.CityTitle,
            dto.ChartTitle
        );

        return Ok(
            await _routeService.GetSavedChartRoutesTitles(
                clientId,
                chartId
            )
        );
    }

    [RequireAnyRole(RoleTypeDTO.SIGNED, RoleTypeDTO.DUTY)]
    [HttpPost("get")]
    public async Task<IActionResult> GetSavedRoute([FromBody] GetSavedRouteRequest dto)
    {
        string? clientIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(clientIdClaim, out int clientId))
            return StatusCode(
                StatusCodes.Status401Unauthorized,
                new
                {
                    Error = "InvalidCredentials",
                    Message = "Не удалось определить пользователя"
                }
            );

        int chartId = await _chartService.GetChartIdAsync(
            dto.CityTitle,
            dto.ChartTitle
        );

        return Ok(
            await _routeService.GetSavedChart(
                clientId,
                chartId,
                dto.RouteTitle
            )
        );
    }

    [RequireAnyRole(RoleTypeDTO.SIGNED, RoleTypeDTO.DUTY)]
    [HttpDelete("route")]
    public async Task<IActionResult> DeleteRoute([FromBody] DeleteRouteRequest dto)
    {
        string? clientIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(clientIdClaim, out int clientId))
            return StatusCode(
                StatusCodes.Status401Unauthorized,
                new
                {
                    Error = "InvalidCredentials",
                    Message = "Не удалось определить пользователя"
                }
            );

        return Ok(
            await _routeService.DeleteAsync(
                clientId,
                dto.RouteTitle
            )
        );
    }
}