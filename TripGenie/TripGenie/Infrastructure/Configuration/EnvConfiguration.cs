namespace TripGenie.API.Infrastructure.Configuration;

public class EnvConfiguration
{
    public DatabaseSettings Database { get; set; } = new();
    public RedisSettings Redis { get; set; } = new();
    public JwtSettings Jwt { get; set; } = new();
}

public class DatabaseSettings
{
    public string ConnectionString { get; set; } = null!;
}

public class RedisSettings
{
    public string ConnectionString { get; set; } = null!;
}

public class JwtSettings
{
    public string Key { get; set; } = null!;
    public string Issuer { get; set; } = null!;
    public string Audience { get; set; } = null!;
    public int DurationInMinutes { get; set; } = 60;
}
