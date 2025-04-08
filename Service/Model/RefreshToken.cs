namespace Service.Model;

public class RefreshToken
{
    public RefreshToken(string token, DateTime expiresAt)
    {
        Token = token;
        ExpiresAt = expiresAt;
    }

    public string Token { get; set; }
    public DateTime ExpiresAt { get; set; }
}
