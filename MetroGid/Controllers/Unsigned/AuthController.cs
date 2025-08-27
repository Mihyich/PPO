using System.Security.Claims;
using MetroGid.Controllers.Utility.DTO;
using MetroGid.Controllers.Utility.DTO.Auth;
using MetroGid.Controllers.Utility.Interfaces;
using MetroGid.Core.Exceptions.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MetroGid.Controllers.Unsigned;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IClientService _clientService;
    private readonly ITokenService _tokenService;

    public AuthController(IClientService clientService, ITokenService tokenService)
    {
        _clientService = clientService;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDTO dto)
    {
        int clientId = await _clientService.RegAsync(dto.Login, dto.Password, dto.Mail);

        RoleTypeDTO role = await _clientService.GetRoleAsync(dto.Login, dto.Password);
        TokenClientDTO tokenClient = new(clientId, dto.Login, role.ToString());
        string token = _tokenService.GenerateToken(tokenClient);

        AuthResponseDTO authResponse = new(clientId, token, tokenClient.Login, tokenClient.Role);
        return Ok(authResponse);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> LogIn([FromBody] LoginRequestDTO dto)
    {
        ClientDTO? client = await _clientService.LogInAsync(dto.Login, dto.Password);

        if (client == null)
            return StatusCode(401, new
            {
                Error = "InvalidCredentials",
                Message = "Неверный логин или пароль"
            });

        int clientId = await _clientService.GetClientIdAsync(dto.Login, dto.Password);
        RoleTypeDTO role = await _clientService.GetRoleAsync(dto.Login, dto.Password);

        TokenClientDTO tokenClient = new(clientId, dto.Login, role.ToString());
        string token = _tokenService.GenerateToken(tokenClient);

        AuthResponseDTO authResponse = new(clientId, token, tokenClient.Login, tokenClient.Role);
        return Ok(authResponse);
    }

    [HttpPost("logout")]
    [Authorize]
    public IActionResult LogOut()
    {
        return Ok(new
        {
            Message = "Вы успешно вышли."
        });
    }

    [HttpDelete("unregister")]
    [Authorize]
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