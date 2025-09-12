using System.Security.Claims;
using MetroGid.Controllers.Attributes;
using MetroGid.Controllers.Utility.Converters;
using MetroGid.Controllers.Utility.DTO.Chart;
using MetroGid.Controllers.Utility.DTO.Concrete;
using MetroGid.Controllers.Utility.Interfaces;
using MetroGid.Core.Converters;
using MCMA = MetroGid.Core.Models.Advanced;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MetroGid.Controllers.Concrete;

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
                DtoDomainConverter.Convert(RoleTypeDTO.DUTY),
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
                DtoDomainConverter.Convert(RoleTypeDTO.DUTY),
                dto.CityTitle, dto.ChartTitle, dto.BranchTitle,
                DtoDomainConverter.Convert(RequestToDTOConverter.Convert(dto))
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

        MCMA.IdRow? stationDutyIdRow = await _chartService.GetStationDutyIdAsync(
            dto.CityTitle, dto.ChartTitle, dto.BranchTitle, dto.StationTitle
        );

        if (stationDutyIdRow == null)
            return StatusCode(
                StatusCodes.Status403Forbidden,
                new
                {
                    Error = "Forbidden",
                    Message = "Указанная станция не обслуживается дежурным"
                }
            );

        if (dutyId != stationDutyIdRow.id)
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
                DtoDomainConverter.Convert(RoleTypeDTO.DUTY),
                dto.CityTitle, dto.ChartTitle, dto.BranchTitle, dto.StationTitle,
                DtoDomainConverter.Convert(RequestToDTOConverter.Convert(dto))
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

        MCMA.IdRow? transitionDutyIdRow = await _chartService.GetTransitionDutyIdAsync(
            dto.CityTitle, dto.ChartTitle,
            dto.FromBranchTitle, dto.FromStationTitle,
            dto.ToBranchTitle, dto.ToStationTitle
        );

        if (transitionDutyIdRow == null)
            return StatusCode(
                StatusCodes.Status403Forbidden,
                new
                {
                    Error = "Forbidden",
                    Message = "Указанный переход не обслуживается дежурным"
                }
            );

        if (dutyId != transitionDutyIdRow.id)
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
                DtoDomainConverter.Convert(RoleTypeDTO.DUTY),
                dto.CityTitle, dto.ChartTitle,
                dto.FromBranchTitle, dto.FromStationTitle,
                dto.ToBranchTitle, dto.ToStationTitle,
                DtoDomainConverter.Convert(RequestToDTOConverter.Convert(dto))
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

        MCMA.IdRow? stationDutyIdRow = await _chartService.GetStationDutyIdAsync(
            dto.CityTitle, dto.ChartTitle, dto.BranchTitle, dto.DutyStationTitle
        );

        if (stationDutyIdRow == null)
            return StatusCode(
                StatusCodes.Status403Forbidden,
                new
                {
                    Error = "Forbidden",
                    Message = "Указанная станция не обслуживается дежурным"
                }
            );

        if (dutyId != stationDutyIdRow.id)
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
                DtoDomainConverter.Convert(RoleTypeDTO.DUTY),
                dto.CityTitle, dto.ChartTitle,
                dto.BranchTitle,
                dto.DutyStationTitle, dto.ToStationTitle,
                DtoDomainConverter.Convert(RequestToDTOConverter.Convert(dto))
            )
        );
    }

    [AllowAnonymous]
    [HttpPost("cities_titles")]
    public async Task<IActionResult> GetChartsCitiesTitles()
    {
        MCMA.ChartIdentifiers ChartsCitiesTitles = await _chartService.GetChartsCitiesTitlesAsync();
        List<ChartDTO> charts = ChartsCitiesTitles.Identifiers.Select(cct => new ChartDTO(cct.Title, cct.City, "")).ToList();
        return Ok(charts);
    }

    [AllowAnonymous]
    [HttpPost("scheme")]
    public async Task<IActionResult> GetScheme([FromBody] GetChartSchemeRequest dto) =>
        Ok(await _chartService.GetChartSchemeAsync(dto.CityTitle, dto.ChartTitle));
}