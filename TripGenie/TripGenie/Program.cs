using Microsoft.EntityFrameworkCore;
using TripGenie.API.Infrastructure.Persistence;
using TripGenie.API.Application.Interfaces;
using TripGenie.API.Application.Services;
using TripGenie.API.Infrastructure.Services;
using TripGenie.API.Infrastructure.Configuration;
using TripGenie.API.Core.Entities;
using TripGenie.API.API.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using StackExchange.Redis;
using Microsoft.OpenApi;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// 1. Load .env file (Development only or based on preference)
var envPath = Path.Combine(Directory.GetCurrentDirectory(), ".env");
if (File.Exists(envPath)) {
    foreach (var line in File.ReadAllLines(envPath)) {
        var parts = line.Split('=', 2, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 2 && !parts[0].StartsWith("#")) {
            Environment.SetEnvironmentVariable(parts[0].Trim(), parts[1].Trim());
        }
    }
}

// 2. Validate & Resolve Configuration (Enterprise Clean Code: Fail Fast)
var envConfig = EnvValidator.Validate(builder.Configuration);
builder.Services.AddSingleton(envConfig);

// Add services to the container.
builder.Services.AddOpenApi(options =>
{
    // Add JWT security scheme to the OpenAPI document
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        // 1. Ensure Components and the SecuritySchemes dictionary are initialized
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();

        document.Components.SecuritySchemes.Add("Bearer", new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "Enter your JWT token in the format: Bearer {token}"
        });
        
        // 2. Ensure the Security requirements list is initialized before adding
        document.Security ??= new List<OpenApiSecurityRequirement>();

        document.Security.Add(new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
        });
        
        return Task.CompletedTask;
    });
});
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();

// Configure EF Core with PostgreSQL (MapEnum inside UseNpgsql handles both EF Core + ADO.NET layers)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(envConfig.Database.ConnectionString, o => o.MapEnum<UserStatus>("user_status"))
           .UseSnakeCaseNamingConvention());

// Configure Redis
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(envConfig.Redis.ConnectionString));

// Register Infrastructure & Data Services
builder.Services.AddScoped<IIdentityRepository, IdentityRepository>();
builder.Services.AddScoped<ISystemService, SystemService>();
builder.Services.AddScoped<ICacheService, CacheService>();
builder.Services.AddScoped<ITokenService, TokenService>();

// Register Application Services
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();


// Configure JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = envConfig.Jwt.Issuer,
        ValidAudience = envConfig.Jwt.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(envConfig.Jwt.Key))
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            // Extract JWT from HttpOnly Cookie for seamless browser support
            context.Token = context.Request.Cookies["access_token"];
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddCustomAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi(); // /openapi/v1.json
    app.UseSwaggerUI(options => 
    {
        options.SwaggerEndpoint("/openapi/v1.json", "TripGenie API v1");
        options.RoutePrefix = "swagger";
    });
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
