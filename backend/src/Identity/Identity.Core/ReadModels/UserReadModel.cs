namespace Identity.Core.ReadModels;

public class UserReadModel:BaseReadModel
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }

    public string Address { get; set; }
    public ICollection<JwtEntity> AccessTokens { get; set; } = new List<JwtEntity>();
}
public class JwtEntity
{
    public string Provider { get; set; } = "PASSWORD";
    public string Token { get; set; }
    public string RefreshToken { get; set; }
}