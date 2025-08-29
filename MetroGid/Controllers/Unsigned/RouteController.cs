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
    IRouteService routeService
) : ControllerBase
{
    private readonly IRouteService _routeService = routeService;

    [AllowAnonymous]
    [HttpPost("search")]
    public async Task<IActionResult> Search([FromBody] SearchRouteRequest dto) =>
        Ok(
            await _routeService.SearchRouteAsync(
                dto.CityTitle, dto.ChartTitle,
                dto.FromBranchTitle, dto.FromStationTitle,
                dto.ToBranchTitle, dto.ToStationTitle,
                TimeConverter.FromString(dto.CurTime)
            )
        );

    // [RequireAnyRole(RoleTypeDTO.SIGNED, RoleTypeDTO.DUTY)]
    // [HttpPost("Save")]
    // public async Task<IActionResult> Save([FromBody] )
}