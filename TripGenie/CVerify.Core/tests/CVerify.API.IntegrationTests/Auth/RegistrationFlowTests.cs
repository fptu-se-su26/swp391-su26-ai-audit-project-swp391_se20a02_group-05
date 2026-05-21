using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using CVerify.API.Application.DTOs;
using CVerify.API.Core.Entities;
using CVerify.API.Infrastructure.Persistence;
using CVerify.API.IntegrationTests.Fixtures;
using CVerify.API.IntegrationTests.Helpers;
using Xunit;

namespace CVerify.API.IntegrationTests.Auth;

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
            Email: "valid@cverify.ai",
            Password: "SecurePassword123!",
            ConfirmPassword: "SecurePassword123!",
            FullName: "Valid User"
        );

        var response = await Client.PostAsJsonAsync("/api/auth/register", request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var data = await response.Content.ReadFromJsonAsync<RegisterResponse>();
        data.Should().NotBeNull();
        data!.StatusCode.Should().Be("REGISTRATION_SUCCESS");
    }

    [Fact]
    public async Task Register_With_Duplicate_Email_Active_Should_Return_Conflict()
    {
        await SeedDefaultRolesAsync();

        var userRequest = new RegisterRequest(
            Email: "valid_active@cverify.ai",
            Password: "SecurePassword123!",
            ConfirmPassword: "SecurePassword123!",
            FullName: "Valid User"
        );

        await Client.PostAsJsonAsync("/api/auth/register", userRequest);

        // Manually activate user
        using (var scope = Factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var user = await db.Users.FirstOrDefaultAsync(u => u.Email == "valid_active@cverify.ai");
            user!.Status = UserStatus.ACTIVE;
            await db.SaveChangesAsync();
        }

        // Try to register same email again
        var response2 = await Client.PostAsJsonAsync("/api/auth/register", userRequest).ConfigureAwait(false);
        response2.StatusCode.Should().Be(HttpStatusCode.Conflict);

        var problem = await response2.Content.ReadFromJsonAsync<Microsoft.AspNetCore.Mvc.ProblemDetails>().ConfigureAwait(false);
        problem.Should().NotBeNull();
        problem!.Extensions.Should().ContainKey("code");
        problem.Extensions["code"]!.ToString().Should().Be(CVerify.API.Application.Exceptions.AuthErrorCodes.EmailAlreadyExists);
    }

    [Fact]
    public async Task Register_With_Duplicate_Email_Pending_Should_Resend_Verification()
    {
        await SeedDefaultRolesAsync();

        var userRequest = new RegisterRequest(
            Email: "valid_pending@cverify.ai",
            Password: "SecurePassword123!",
            ConfirmPassword: "SecurePassword123!",
            FullName: "Valid User"
        );

        await Client.PostAsJsonAsync("/api/auth/register", userRequest);

        // Try to register same email again without activating
        var response2 = await Client.PostAsJsonAsync("/api/auth/register", userRequest).ConfigureAwait(false);
        response2.StatusCode.Should().Be(HttpStatusCode.OK);

        var data2 = await response2.Content.ReadFromJsonAsync<RegisterResponse>().ConfigureAwait(false);
        data2.Should().NotBeNull();
        data2!.StatusCode.Should().Be("REGISTRATION_PENDING_VERIFY");
        data2.UiAction.Should().Be("SHOW_WARNING_TOAST");
    }

    [Fact]
    public async Task Register_With_Weak_Password_Should_Return_BadRequest()
    {
        await SeedDefaultRolesAsync();

        var request = new RegisterRequest(
            Email: "weak@cverify.ai",
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
            Email: "mismatch@cverify.ai",
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
            Email: "  NoRmAlIzE@cVeRiFy.aI   ",
            Password: "SecurePassword123!",
            ConfirmPassword: "SecurePassword123!",
            FullName: "Normalized User"
        );

        var response = await Client.PostAsJsonAsync("/api/auth/register", request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == "normalize@cverify.ai");
        user.Should().NotBeNull();
    }
}
