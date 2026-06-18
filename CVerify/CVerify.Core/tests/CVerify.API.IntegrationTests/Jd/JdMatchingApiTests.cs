using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using CVerify.API.Modules.Auth.Entities;
using CVerify.API.IntegrationTests.Fixtures;
using CVerify.API.Modules.Jd.DTOs;
using CVerify.API.Modules.Shared.Domain.Entities;
using CVerify.API.Modules.Shared.Domain.Enums;
using CVerify.API.Modules.Shared.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace CVerify.API.IntegrationTests.Jd;

public sealed class JdMatchingApiTests : BaseIntegrationTest
{
    public JdMatchingApiTests(SharedTestcontainerFixture containerFixture) : base(containerFixture)
    {
    }

    [Fact]
    public async Task MatchCandidate_ShouldReturnQualityGateReport_ForAuthenticatedUser()
    {
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Add("Cookie", await CreateSessionCookieAsync("jd-match@cverify.ai"));

        var request = new JdMatchRequest(
            new JdFormRequest(
                "Senior Software Engineer",
                "Senior",
                ["React", "TypeScript"],
                ["PostgreSQL"],
                ["Design and implement REST APIs", "Build frontend features"],
                3,
                6,
                "Bachelor's Degree",
                "Advanced",
                2000m,
                3000m,
                "USD",
                "Ho Chi Minh City",
                "hybrid"),
            [
                new CandidateSkillEvidence("ReactJS", 5m, "strong"),
                new CandidateSkillEvidence("TypeScript", 4m, "strong")
            ],
            ["Design and implement REST APIs", "Build frontend features with React"],
            "L3",
            2500m,
            2000m,
            "USD",
            "Engineer",
            ["hybrid"]);

        var response = await client.PostAsJsonAsync("/api/jd/match", request);

        var responseBody = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, responseBody);
        var result = await response.Content.ReadFromJsonAsync<MatchScoreResponse>();
        result.Should().NotBeNull();
        result!.QualityGate.CanApply.Should().BeTrue();
        result.HiringRecommendation.Verdict.Should().Be("Yes");
    }

    private async Task<string> CreateSessionCookieAsync(string email)
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        if (!await db.Roles.AnyAsync(r => r.Name == "USER"))
        {
            db.Roles.Add(new Role
            {
                Id = Guid.Parse("018fc35b-1c5d-7b8a-9a2d-3e4f5a6b7c8d"),
                Name = "USER",
                DisplayName = "General User",
                Description = "Basic application access",
                IsSystem = true,
                IsActive = true
            });
            await db.SaveChangesAsync();
        }

        var user = new User
        {
            Id = Guid.CreateVersion7(),
            Email = email,
            FullName = "JD Match Test User",
            PasswordHash = "test-hash",
            Status = UserStatus.ACTIVE,
            EmailVerifiedAt = DateTimeOffset.UtcNow,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var sessionId = Guid.NewGuid();
        var refreshTokenValue = $"jd_match_rt_{Guid.NewGuid()}";
        db.RefreshTokens.Add(new RefreshToken
        {
            UserId = user.Id,
            Token = refreshTokenValue,
            SessionId = sessionId,
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(7)
        });
        await db.SaveChangesAsync();

        var jwt = CreateJwt(user, sessionId);
        return $"access_token={jwt}; refresh_token={refreshTokenValue}";
    }

    private static string CreateJwt(User user, Guid sessionId)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Role, "USER"),
            new Claim("isEmailVerified", "true"),
            new Claim("status", UserStatus.ACTIVE.ToString()),
            new Claim("session_version", user.SessionVersion.ToString()),
            new Claim("sid", sessionId.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("super_secret_key_super_secret_key_super_secret_key_32_characters"));
        var token = new JwtSecurityToken(
            issuer: "CVerify.API",
            audience: "CVerify.Client",
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
