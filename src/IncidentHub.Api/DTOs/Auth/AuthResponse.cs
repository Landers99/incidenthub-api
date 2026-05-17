namespace IncidentHub.Api.DTOs.Auth;

public class AuthResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; set; }

    public CurrentUserResponse User { get; set; } = default!;
}
