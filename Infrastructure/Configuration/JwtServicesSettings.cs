namespace Infrastructure.Configuration;

public class JwtServicesConfig
{
    public string Issuer { get; set; }
    public int SecondsUntilExpire { get; set; }
    public Dictionary<string, JwtServiceCredentials> JwtServices { get; set; }
}

public class JwtServiceCredentials
{
    public string ServiceId { get; set; }
    public string ServiceSecret { get; set; }
}
