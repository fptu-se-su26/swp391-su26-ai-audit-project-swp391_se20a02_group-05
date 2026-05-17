using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TripGenie.API.Application.DTOs;
using TripGenie.API.Core.Entities;
using TripGenie.API.Infrastructure.Persistence;
using TripGenie.API.IntegrationTests.Fixtures;
using TripGenie.API.IntegrationTests.Helpers;
using Xunit;

namespace TripGenie.API.IntegrationTests.Auth;

public class RegistrationFlowTests : BaseIntegrationTest
{
    public RegistrationFlowTests(SharedTestcontainerFixture containerFixture) : base(containerFixture)
    {
    }

    private async Task SeedDefaultRolesAsync()
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userRole = await db.Roles.FirstOrDefaultAsync(r => r.Name == "USER");
        if (userRole == null)
        {
            db.Roles.Add(new Role
            {
                Name = "USER",
                DisplayName = "General User",
                Description = "Basic application access",
                IsSystem = true,
                IsActive = true
            });
            await db.SaveChangesAsync();
        }
    }

    [Fact]
    public async Task Register_With_Valid_Inputs_Should_Return_Success()
    {
        await SeedDefaultRolesAsync();

        var request = new RegisterRequest(
            Email: "valid@tripgenie.ai",
            Password: "SecurePassword123!",
            ConfirmPassword: "SecurePassword123!",
            FullName: "Valid User"
        );

        var response = await Client.PostAsJsonAsync("/api/auth/register", request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Register_With_Weak_Password_Should_Return_BadRequest()
    {
        await SeedDefaultRolesAsync();

        var request = new RegisterRequest(
            Email: "weak@tripgenie.ai",
            Password: "123",
            ConfirmPassword: "123",
            FullName: "Weak User"
        );

        var response = await Client.PostAsJsonAsync("/api/auth/register", request);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Register_With_Mismatched_ConfirmPassword_Should_Return_BadRequest()
    {
        await SeedDefaultRolesAsync();

        var request = new RegisterRequest(
            Email: "mismatch@tripgenie.ai",
            Password: "SecurePassword123!",
            ConfirmPassword: "DifferentPassword123!",
            FullName: "Mismatch User"
        );

        var response = await Client.PostAsJsonAsync("/api/auth/register", request);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Register_Should_Normalize_Email()
    {
        await SeedDefaultRolesAsync();

        var request = new RegisterRequest(
            Email: "  NoRmAlIzE@TrIpGeNiE.aI   ",
            Password: "SecurePassword123!",
            ConfirmPassword: "SecurePassword123!",
            FullName: "Normalized User"
        );

        var response = await Client.PostAsJsonAsync("/api/auth/register", request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == "normalize@tripgenie.ai");
        user.Should().NotBeNull();
    }
}
