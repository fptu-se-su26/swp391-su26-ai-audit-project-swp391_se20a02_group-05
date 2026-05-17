namespace TripGenie.API.Infrastructure.Configuration;

public class EnvConfiguration
{
    public DatabaseSettings Database { get; set; } = new();
    public RedisSettings Redis { get; set; } = new();
    public JwtSettings Jwt { get; set; } = new();
    public AuthSettings Auth { get; set; } = new();
    public AuthRateLimitSettings RateLimit { get; set; } = new();
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

public class AuthSettings
{
    public int VerificationTokenDurationInHours { get; set; } = 24;
    public int ResetPasswordTokenDurationInMinutes { get; set; } = 30;
    public string ResetPasswordUrlFormat { get; set; } = "https://tripgenie.ai/reset?token={token}";
    public string VerifyEmailUrlFormat { get; set; } = "https://tripgenie.ai/verify?token={token}";
    public string TrustedDomains { get; set; } = "tripgenie.ai;localhost;127.0.0.1";
}

public class AuthRateLimitSettings
{
    public int ForgotPasswordPermitLimit { get; set; } = 3;
    public int ForgotPasswordWindowMinutes { get; set; } = 15;
    
    public int ResetPasswordPermitLimit { get; set; } = 5;
    public int ResetPasswordWindowMinutes { get; set; } = 15;

    public int ResendVerificationPermitLimit { get; set; } = 3;
    public int ResendVerificationWindowMinutes { get; set; } = 5;

    public int VerifyEmailPermitLimit { get; set; } = 10;
    public int VerifyEmailWindowMinutes { get; set; } = 5;
}
