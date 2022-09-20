namespace Identity.Application.Dto.Users.Response;

public class ResponseBase
{
    public long Id { get; set; }
    public bool Success { get; set; } = true;
}