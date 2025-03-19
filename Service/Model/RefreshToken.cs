namespace CulturalShare.Auth.Services.Model;

public class RefreshToken
{
    public string Token { get; set; }
    public DateTime IssuedAt { get; set; }
}
