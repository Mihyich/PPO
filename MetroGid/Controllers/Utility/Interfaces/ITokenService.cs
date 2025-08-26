using MetroGid.Controllers.Utility.DTO.Auth;

namespace MetroGid.Controllers.Utility.Interfaces;

public interface ITokenService
{
    string GenerateToken(TokenClientDTO client);
}