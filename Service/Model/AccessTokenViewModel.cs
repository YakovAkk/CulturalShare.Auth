namespace Service.Model;

public class AccessTokenViewModel
{
    public string AccessToken { get; set; }
    public DateTime AccessTokenExpiresAt { get; set; }

    public AccessTokenViewModel(string accessToken, DateTime accessTokenExpiresAt)
    {
        AccessToken = accessToken;
        AccessTokenExpiresAt = accessTokenExpiresAt;
    }
}
