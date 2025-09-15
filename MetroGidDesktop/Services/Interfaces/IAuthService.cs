using System.Threading.Tasks;
using MetroGid.Controllers.Utility.DTO.Auth;

namespace MetroGidDesktop.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDTO> RegisterAsync(RegisterRequestDTO dto);
    Task<AuthResponseDTO> LogInAsync(LoginRequestDTO dto);
    Task<bool> LogOutAsync();
    Task<bool> UnregisterAsync(UnregisterRequestDto dto);
}