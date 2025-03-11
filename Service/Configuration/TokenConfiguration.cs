namespace CulturalShare.Auth.Services.Configuration;

public class TokenConfiguration
{
    public string AuthorizationKey { get; set; }
    public string OneTimeAuthorizationKey { get; set; }
    public int DaysUntilExpire { get; set; }
}
