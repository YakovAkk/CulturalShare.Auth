namespace CulturalShare.Auth.Services.Model;

public class RefreshToken
{
    public string Token { get; set; }
    public DateTime ExpiresAt { get; set; }
}
