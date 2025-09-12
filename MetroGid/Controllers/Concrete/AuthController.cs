using System.Security.Claims;
using MCUD = MetroGid.Controllers.Utility.DTO.Concrete;
using MetroGid.Controllers.Utility.DTO.Auth;
using MetroGid.Controllers.Utility.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MetroGid.Controllers.Utility.Converters;
using MCMC = MetroGid.Core.Models.Concrete;
using MCMT = MetroGid.Core.Models.Types;
using MCMA = MetroGid.Core.Models.Advanced;
using MetroGid.Core.Converters;

namespace MetroGid.Controllers.Concrete;

[ApiController]
[Route("api/auth")]
public class AuthController(
    IClientService clientService,
    ITokenService tokenService
) : ControllerBase
{
    private readonly IClientService _clientService = clientService;
    private readonly ITokenService _tokenService = tokenService;

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDTO dto)
    {
        MCMA.IdRow idRow = await _clientService.RegAsync(dto.Login, dto.Password, dto.Mail);
        AuthResponseDTO authResponse = new(idRow.id, "", dto.Login, "");
        return Ok(authResponse);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> LogIn([FromBody] LoginRequestDTO dto)
    {
        MCMC.Client? mcmcClient = await _clientService.LogInAsync(dto.Login, dto.Password);

        if (mcmcClient == null)
            return StatusCode(
                StatusCodes.Status401Unauthorized,
                new
                {
                    Error = "InvalidCredentials",
                    Message = "Неверный логин или пароль"
                }
            );

        MCUD.ClientDTO mcudClient = DomainDtoConverter.Convert(mcmcClient);

        MCMA.IdRow idRow = await _clientService.GetClientIdAsync(dto.Login, dto.Password);

        TokenClientDTO tokenClient = new(idRow.id, dto.Login, DTORoleToDBRoleConverter.Convert(mcudClient.Role));
        string token = _tokenService.GenerateToken(tokenClient);

        AuthResponseDTO authResponse = new(idRow.id, token, tokenClient.Login, tokenClient.Role);
        return Ok(authResponse);
    }

    [Authorize]
    [HttpPost("logout")]
    public IActionResult LogOut()
    {
        return Ok(new
            {
                Message = "Вы успешно вышли"
            }
        );
    }

    [Authorize]
    [HttpDelete("unregister")]
    public async Task<IActionResult> Unregister([FromBody] UnregisterRequestDto dto)
    {
        string? clientIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(clientIdClaim, out int userId))
            return StatusCode(401, new
            {
                Error = "InvalidCredentials",
                Message = "Не удалось определить пользователя"
            });

        if (!await _clientService.VerifyPasswordAsync(userId, dto.Password))
            return StatusCode(401, new
            {
                Error = "InvalidCredentials",
                Message = "Неверный логин или пароль"
            });

        await _clientService.UnRegAsync(userId);

        return Ok(new { Message = "Ваш аккаунт успешно удалён" });
    }
}