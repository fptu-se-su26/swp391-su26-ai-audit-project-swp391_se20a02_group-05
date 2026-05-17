using TripGenie.API.API.Extensions;

namespace TripGenie.API.Infrastructure.Configuration;

public static class EnvValidator
{
    public static EnvConfiguration Validate(IConfiguration configuration)
    {
        var config = new EnvConfiguration();

        // 1. Database
        config.Database.ConnectionString = configuration.GetConnectionString("DefaultConnection")?.ResolveEnvironmentVariables()
            ?? throw new InvalidOperationException("Environment variable 'DATABASE_URL' is missing or connection string 'DefaultConnection' is not configured.");

        // 2. Redis
        config.Redis.ConnectionString = configuration.GetConnectionString("Redis")?.ResolveEnvironmentVariables()
            ?? throw new InvalidOperationException("Environment variable 'REDIS_URL' is missing or connection string 'Redis' is not configured.");

        // 3. JWT
        config.Jwt.Key = configuration["Jwt:Key"]?.ResolveEnvironmentVariables() ?? throw new InvalidOperationException("Environment variable 'JWT_KEY' is missing.");
        config.Jwt.Issuer = configuration["Jwt:Issuer"]?.ResolveEnvironmentVariables() ?? throw new InvalidOperationException("Environment variable 'JWT_ISSUER' is missing.");
        config.Jwt.Audience = configuration["Jwt:Audience"]?.ResolveEnvironmentVariables() ?? throw new InvalidOperationException("Environment variable 'JWT_AUDIENCE' is missing.");
        
        if (int.TryParse(configuration["Jwt:DurationInMinutes"], out var duration))
        {
            config.Jwt.DurationInMinutes = duration;
        }

        return config;
    }
}
