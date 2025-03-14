namespace Service.Services.Base;

public interface IJwtBlacklistService
{
    Task<bool> IsTokenBlacklistedAsync(string jti);
    Task BlacklistTokenAsync(string jti, TimeSpan expiry);
    Task RemoveFromBlacklistAsync(string jti);

}
