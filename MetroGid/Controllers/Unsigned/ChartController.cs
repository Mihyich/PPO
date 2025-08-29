using System.Security.Claims;
using MetroGid.Controllers.Attributes;
using MetroGid.Controllers.Utility.Converters;
using MetroGid.Controllers.Utility.DTO.Chart;
using MetroGid.Controllers.Utility.DTO.Concrete;
using MetroGid.Controllers.Utility.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MetroGid.Controllers.Unsigned;

[ApiController]
[Route("api/chart")]
public class ChartController(
    IChartService chartService
) : ControllerBase
{
    private readonly IChartService _chartService = chartService;

    [AllowAnonymous]
    [HttpPost("add")]
    public async Task<IActionResult> AddChart([FromBody] AddChartRequest dto) =>
        Ok(await _chartService.AddChartAsync(dto.ChartJson));

    [RequireAnyRole(RoleTypeDTO.DUTY)]
    [HttpPatch("scheme")]
    public async Task<IActionResult> PatchChartScheme([FromBody] PatchChartSchemeRequest dto) =>
        Ok(
            await _chartService.UpdateChartSchemeAsync(
                RoleTypeDTO.DUTY,
                dto.CityTitle,
                dto.ChartTitle,
                dto.Scheme
            )
        );

    [RequireAnyRole(RoleTypeDTO.DUTY)]
    [HttpPatch("branch")]
    public async Task<IActionResult> PatchBranch([FromBody] PatchBranchRequest dto) =>
        Ok(
            await _chartService.UpdateBranchAsync(
                RoleTypeDTO.DUTY,
                dto.CityTitle, dto.ChartTitle, dto.BranchTitle,
                RequestToDTOConverter.Convert(dto)
            )
        );

    [RequireAnyRole(RoleTypeDTO.DUTY)]
    [HttpPatch("station")]
    public async Task<IActionResult> PatchStation([FromBody] PatchStationRequest dto)
    {
        string? dutyIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(dutyIdClaim, out int dutyId))
            return StatusCode(
                StatusCodes.Status401Unauthorized,
                new
                {
                    Error = "InvalidCredentials",
                    Message = "Не удалось определить пользователя"
                }
            );

        int? stationDutyId = await _chartService.GetStationDutyIdAsync(
            dto.CityTitle, dto.ChartTitle, dto.BranchTitle, dto.StationTitle
        );

        if (stationDutyId == null)
            return StatusCode(
                StatusCodes.Status403Forbidden,
                new
                {
                    Error = "Forbidden",
                    Message = "Указанная станция не обслуживается дежурным"
                }
            );

        if (dutyId != stationDutyId)
            return StatusCode(
                StatusCodes.Status403Forbidden,
                new
                {
                    Error = "Forbidden",
                    Message = "Вы не являетесь дежурным указанной станции"
                }
            );

        return Ok(
            await _chartService.UpdateStationAsync(
                RoleTypeDTO.DUTY,
                dto.CityTitle, dto.ChartTitle, dto.BranchTitle, dto.StationTitle,
                RequestToDTOConverter.Convert(dto)
            )
        );
    }

    [RequireAnyRole(RoleTypeDTO.DUTY)]
    [HttpPatch("transition")]
    public async Task<IActionResult> PatchTransition([FromBody] PatchTransitionRequest dto)
    {
        string? dutyIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(dutyIdClaim, out int dutyId))
            return StatusCode(
                StatusCodes.Status401Unauthorized,
                new
                {
                    Error = "InvalidCredentials",
                    Message = "Не удалось определить пользователя"
                }
            );

        int? transitionDutyId = await _chartService.GetTransitionDutyIdAsync(
            dto.CityTitle, dto.ChartTitle,
            dto.FromBranchTitle, dto.FromStationTitle,
            dto.ToBranchTitle, dto.ToStationTitle
        );

        if (transitionDutyId == null)
            return StatusCode(
                StatusCodes.Status403Forbidden,
                new
                {
                    Error = "Forbidden",
                    Message = "Указанный переход не обслуживается дежурным"
                }
            );

        if (dutyId != transitionDutyId)
            return StatusCode(
                StatusCodes.Status403Forbidden,
                new
                {
                    Error = "Forbidden",
                    Message = "Вы не являетесь дежурным указанного перехода"
                }
            );

        return Ok(
            await _chartService.UpdateTransitionAsync(
                RoleTypeDTO.DUTY,
                dto.CityTitle, dto.ChartTitle,
                dto.FromBranchTitle, dto.FromStationTitle,
                dto.ToBranchTitle, dto.ToStationTitle,
                RequestToDTOConverter.Convert(dto)
            )
        );
    }

    [RequireAnyRole(RoleTypeDTO.DUTY)]
    [HttpPatch("railway")]
    public async Task<IActionResult> PatchRailway([FromBody] PatchRailwayRequest dto)
    {
        string? dutyIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(dutyIdClaim, out int dutyId))
            return StatusCode(
                StatusCodes.Status401Unauthorized,
                new
                {
                    Error = "InvalidCredentials",
                    Message = "Не удалось определить пользователя"
                }
            );

        int? stationDutyId = await _chartService.GetStationDutyIdAsync(
            dto.CityTitle, dto.ChartTitle, dto.BranchTitle, dto.DutyStationTitle
        );

        if (stationDutyId == null)
            return StatusCode(
                StatusCodes.Status403Forbidden,
                new
                {
                    Error = "Forbidden",
                    Message = "Указанная станция не обслуживается дежурным"
                }
            );

        if (dutyId != stationDutyId)
            return StatusCode(
                StatusCodes.Status403Forbidden,
                new
                {
                    Error = "Forbidden",
                    Message = "Вы не являетесь дежурным указанной станции"
                }
            );

        return Ok(
            await _chartService.UpdateRailwayAsync(
                RoleTypeDTO.DUTY,
                dto.CityTitle, dto.ChartTitle,
                dto.BranchTitle,
                dto.DutyStationTitle, dto.ToStationTitle,
                RequestToDTOConverter.Convert(dto)
            )
        );
    }

    [AllowAnonymous]
    [HttpPost("cities_titles")]
    public async Task<IActionResult> GetChartsCitiesTitles()
    {
        List<(string, string)> ChartsCitiesTitles = await _chartService.GetChartsCitiesTitlesAsync();
        List<ChartDTO> charts = ChartsCitiesTitles.Select(cct => new ChartDTO(cct.Item1, cct.Item2, "")).ToList();
        return Ok(charts);
    }

    [AllowAnonymous]
    [HttpPost("scheme")]
    public async Task<IActionResult> GetScheme([FromBody] GetChartSchemeRequest dto) =>
        Ok(await _chartService.GetChartSchemeAsync(dto.CityTitle, dto.ChartTitle));
}