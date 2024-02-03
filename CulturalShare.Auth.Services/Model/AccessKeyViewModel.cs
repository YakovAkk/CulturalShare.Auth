namespace CulturalShare.Auth.Services.Model;

public class AccessKeyViewModel
{
    public string AccessToken { get; set; } // secret token
    public string RefreshToken { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpireDate { get; set; }

    public AccessKeyViewModel()
    {
        CreatedAt = DateTime.UtcNow;
    }
}
