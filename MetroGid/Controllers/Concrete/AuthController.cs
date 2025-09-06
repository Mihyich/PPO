using System.Security.Claims;
using MetroGid.Controllers.Utility.DTO.Concrete;
using MetroGid.Controllers.Utility.DTO.Auth;
using MetroGid.Controllers.Utility.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MetroGid.Controllers.Utility.Converters;

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
        int clientId = await _clientService.RegAsync(dto.Login, dto.Password, dto.Mail);
        AuthResponseDTO authResponse = new(clientId, "", dto.Login, "");
        return Ok(authResponse);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> LogIn([FromBody] LoginRequestDTO dto)
    {
        ClientDTO? client = await _clientService.LogInAsync(dto.Login, dto.Password);

        if (client == null)
            return StatusCode(
                StatusCodes.Status401Unauthorized,
                new
                {
                    Error = "InvalidCredentials",
                    Message = "Неверный логин или пароль"
                }
            );

        int clientId = await _clientService.GetClientIdAsync(dto.Login, dto.Password);
        RoleTypeDTO role = await _clientService.GetRoleAsync(dto.Login, dto.Password);

        TokenClientDTO tokenClient = new(clientId, dto.Login, DTORoleToDBRoleConverter.Convert(role));
        string token = _tokenService.GenerateToken(tokenClient);

        AuthResponseDTO authResponse = new(clientId, token, tokenClient.Login, tokenClient.Role);
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