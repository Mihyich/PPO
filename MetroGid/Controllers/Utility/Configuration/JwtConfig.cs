namespace MetroGid.Controllers.Utility.Configuration;

public class JwtConfig
{
    public string Key { get; set; } = null!;
    public string Issuer { get; set; } = null!;
    public string Audience { get; set; } = null!;
    public Dictionary<string, int> TokenLifetimeMinutes { get; set; } = new();
}