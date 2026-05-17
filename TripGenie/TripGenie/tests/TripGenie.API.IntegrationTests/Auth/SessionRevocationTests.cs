using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TripGenie.API.Application.DTOs;
using TripGenie.API.Application.Exceptions;
using TripGenie.API.Core.Entities;
using TripGenie.API.Infrastructure.Persistence;
using TripGenie.API.IntegrationTests.Fixtures;
using TripGenie.API.IntegrationTests.Helpers;
using Xunit;

namespace TripGenie.API.IntegrationTests.Auth;

public class SessionRevocationTests : BaseIntegrationTest
{
    public SessionRevocationTests(SharedTestcontainerFixture containerFixture) : base(containerFixture)
    {
    }

    private async Task<Guid> CreateActiveUserAsync(string email)
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var userRole = await db.Roles.FirstOrDefaultAsync(r => r.Name == "USER");
        if (userRole == null)
        {
            userRole = new Role
            {
                Name = "USER",
                DisplayName = "General User",
                Description = "Basic app user",
                IsSystem = true,
                IsActive = true
            };
            db.Roles.Add(userRole);
            await db.SaveChangesAsync();
        }

        var user = new UserBuilder()
            .WithEmail(email)
            .WithStatus(UserStatus.ACTIVE)
            .WithRole(userRole)
            .Build();

        db.Users.Add(user);
        await db.SaveChangesAsync();
        return user.Id;
    }

    [Fact]
    public async Task RefreshTokenTheft_Should_Revoke_All_User_Sessions_And_Throw_InvalidToken()
    {
        var email = $"theft_{Guid.NewGuid()}@tripgenie.ai";
        var userId = await CreateActiveUserAsync(email);

        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Create active and revoked tokens for this user
        var revokedToken = new RefreshToken
        {
            UserId = userId,
            Token = "already_revoked_token_value_abc",
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(7),
            RevokedAt = DateTimeOffset.UtcNow.AddMinutes(-5)
        };
        db.RefreshTokens.Add(revokedToken);

        var activeToken = new RefreshToken
        {
            UserId = userId,
            Token = "currently_active_token_value_xyz",
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(7)
        };
        db.RefreshTokens.Add(activeToken);

        await db.SaveChangesAsync();

        // Simulate presenting the already revoked token (theft scenario)
        Client.DefaultRequestHeaders.Add("Cookie", "refresh_token=already_revoked_token_value_abc");

        var response = await Client.PostAsync("/api/auth/refresh-token", null);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);


        var problem = await response.Content.ReadFromJsonAsync<Microsoft.AspNetCore.Mvc.ProblemDetails>();
        problem.Should().NotBeNull();
        problem!.Extensions["code"]!.ToString().Should().Be(AuthErrorCodes.InvalidToken);

        // Verify ALL refresh tokens for this user are now revoked in the DB
        using var scope2 = Factory.Services.CreateScope();
        var db2 = scope2.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var activeTokens = await db2.RefreshTokens.Where(t => t.UserId == userId && t.RevokedAt == null).ToListAsync();
        activeTokens.Should().BeEmpty();
    }
}
