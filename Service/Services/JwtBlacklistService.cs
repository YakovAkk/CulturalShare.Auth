using Service.Services.Base;
using StackExchange.Redis;

namespace Service.Services;

public class JwtBlacklistService : IJwtBlacklistService
{
    private const string BlacklistPrefix = "jwt:blacklist:";

    private readonly IDatabase _redisDb;

    public JwtBlacklistService(IConnectionMultiplexer redis)
    {
        _redisDb = redis.GetDatabase();
    }

    /// <summary>
    /// Checks if a JWT token is blacklisted.
    /// </summary>
    /// <param name="jti">JWT ID (jti) claim</param>
    /// <returns>True if the token is blacklisted, otherwise false.</returns>
    public async Task<bool> IsTokenBlacklistedAsync(string jti)
    {
        return await _redisDb.KeyExistsAsync(BlacklistPrefix + jti);
    }

    /// <summary>
    /// Blacklists a JWT token until its expiration time.
    /// </summary>
    /// <param name="jti">JWT ID (jti) claim</param>
    /// <param name="expiry">Expiration time of the token</param>
    public async Task BlacklistTokenAsync(string jti, TimeSpan expiry)
    {
        await _redisDb.StringSetAsync(BlacklistPrefix + jti, "revoked", expiry);
    }

    /// <summary>
    /// Removes a JWT token from the blacklist manually.
    /// </summary>
    /// <param name="jti">JWT ID (jti) claim</param>
    public async Task RemoveFromBlacklistAsync(string jti)
    {
        await _redisDb.KeyDeleteAsync(BlacklistPrefix + jti);
    }
}