namespace Service.Model;

public class AccessAndRefreshTokenViewModel : AccessTokenViewModel
{
    public AccessAndRefreshTokenViewModel(
        string accessToken,
        DateTime accessTokenExpiresAt,
        string refreshToken,
        DateTime refreshTokenExpiresAt) : base(accessToken, accessTokenExpiresAt)
    {
        RefreshToken = refreshToken;
        RefreshTokenExpiresAt = refreshTokenExpiresAt;
    }

    public string RefreshToken { get; set; }
    public DateTime RefreshTokenExpiresAt { get; set; }
}
