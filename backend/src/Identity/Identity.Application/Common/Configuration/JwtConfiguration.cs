namespace Identity.Application.Common.Configuration;

public class JwtConfiguration
{
    public string Audience { get; set; } = String.Empty;
    public string Issuer { get; set; } = String.Empty;
    public string SymmetricSecurityKey { get; set; } = String.Empty;
    public int Expires { get; set; } = 240;
    public int RefreshTokenExpires { get; set; } = 240;
}