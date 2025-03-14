namespace Infrastructure.Configuration;

public class JwtServicesConfig
{
    public string Issuer { get; set; }
    public int SecondsUntilExpireUserKey { get; set; }
    public int SecondsUntilExpireServiceKey { get; set; }
    public Dictionary<string, string> JwtSecretTokenPairs { get; set; }
}

public class JwtServiceCredentials
{
    public string ServiceId { get; set; }
    public string ServiceSecret { get; set; }
}
