using System.IdentityModel.Tokens.Jwt;
using System.Security;
using System.Security.Claims;
using System.Text;
using MetroGid.Controllers.Utility.Configuration;
using MetroGid.Controllers.Utility.DTO.Auth;
using MetroGid.Controllers.Utility.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace MetroGid.Core.Services;

public class JwtTokenService(IOptions<JwtConfig> JwtConfig) : ITokenService
{
    private readonly JwtConfig _JwtConfig = JwtConfig.Value;

    public string GenerateToken(TokenClientDTO client)
    {
        if (!_JwtConfig.TokenLifetimeMinutes.ContainsKey(client.Role))
            throw new SecurityException($"Роль {client.Role} не поддерживается токенами");

        Claim[] claims = 
        {
            new Claim(ClaimTypes.NameIdentifier, client.Id.ToString()),
            new Claim(ClaimTypes.Name, client.Login),
            new Claim(ClaimTypes.Role, client.Role)
        };

        SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(_JwtConfig.Key));
        SigningCredentials creds = new(key, SecurityAlgorithms.HmacSha256);
        int lifetimeMinutes = _JwtConfig.TokenLifetimeMinutes.GetValueOrDefault(client.Role);

        JwtSecurityToken token = new
        (
            issuer: _JwtConfig.Issuer,
            audience: _JwtConfig.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(lifetimeMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}