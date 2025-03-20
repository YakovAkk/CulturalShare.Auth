using CulturalShare.Auth.Services.Model;

namespace Service.Model;

public class AccessAndRefreshTokenViewModel : AccessTokenViewModel
{
    public string RefreshToken { get; set; }
    public DateTime RefreshTokenExpiresAt { get; set; }
}
