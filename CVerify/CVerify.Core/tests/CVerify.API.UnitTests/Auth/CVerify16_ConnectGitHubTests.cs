using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using CVerify.API.Modules.Auth.Controllers;
using CVerify.API.Modules.Auth.Entities;
using CVerify.API.Modules.Auth.Services;
using CVerify.API.Modules.Shared.Configuration;
using CVerify.API.Modules.Shared.Domain.Entities;
using CVerify.API.Modules.Shared.Domain.Enums;
using CVerify.API.Modules.Shared.Persistence;

namespace CVerify.API.UnitTests.Auth;

/// <summary>
/// Unit tests for AuthController.ConnectProvider("github") — CVerify-16 (6 UTCIDs).
/// GET /api/auth/connect/github — initiates GitHub OAuth flow.
/// </summary>
public sealed class CVerify16_ConnectGitHubTests : IDisposable
{
    private readonly ServiceProvider _sp;
    private readonly ApplicationDbContext _context;

    private readonly Mock<IAuthService>                  _authService   = new();
    private readonly Mock<IIdentityStateResolver>        _idResolver    = new();
    private readonly Mock<IWorkspaceProvisioningService> _workspaceProv = new();

    private readonly AuthController _sut;

    public CVerify16_ConnectGitHubTests()
    {
        var services = new ServiceCollection();
        services.AddDbContext<ApplicationDbContext>(opts =>
            opts.UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning)));
        // Register EnvConfiguration with a test GitHub client ID
        services.AddSingleton(new EnvConfiguration
        {
            Auth = new AuthSettings
            {
                GithubClientId = "test_github_client_id"
            }
        });
        _sp = services.BuildServiceProvider();
        _context = _sp.GetRequiredService<ApplicationDbContext>();

        _sut = new AuthController(
            _authService.Object,
            _idResolver.Object,
            new Mock<ILogger<AuthController>>().Object,
            _workspaceProv.Object);
    }

    public void Dispose() => _sp.Dispose();

    private void SetupUser(Guid userId)
    {
        var ctx = new DefaultHttpContext { RequestServices = _sp };
        ctx.User = new ClaimsPrincipal(new ClaimsIdentity(
            new[] { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) }, "Test"));
        ctx.Request.Scheme = "https";
        ctx.Request.Host   = new HostString("api.example.com");
        _sut.ControllerContext = new ControllerContext { HttpContext = ctx };
    }

    private async Task<User> SeedUserAsync()
    {
        var user = new User
        {
            Id = Guid.NewGuid(), Email = "user@example.com", FullName = "Test User",
            Username = "testuser", Status = UserStatus.ACTIVE, EmailVerifiedAt = DateTime.UtcNow,
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    // ── UTCID01 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify16_UTCID01_ConnectGitHub_ValidUser_NoExistingLinks_ReturnsRedirectToGitHub()
    {
        var user = await SeedUserAsync();
        SetupUser(user.Id);

        var result = await _sut.ConnectProvider("github");

        result.Should().BeOfType<RedirectResult>()
            .Which.Url.Should().Contain("github.com/login/oauth/authorize");
    }

    // ── UTCID02 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify16_UTCID02_ConnectGitHub_UserHasOneLinkedGitHub_ReturnsRedirect()
    {
        var user = await SeedUserAsync();
        _context.AuthProviders.Add(new AuthProvider
        {
            Id = Guid.NewGuid(), UserId = user.Id,
            ProviderName = "github", ProviderKey = "gh_uid_1",
            ScopeValidationStatus = ProviderScopeStatus.Valid,
        });
        await _context.SaveChangesAsync();
        SetupUser(user.Id);

        var result = await _sut.ConnectProvider("github");

        result.Should().BeOfType<RedirectResult>();
    }

    // ── UTCID03 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify16_UTCID03_ConnectGitHub_UserAlreadyHasThreeLinkedAccounts_Returns400BadRequest()
    {
        var user = await SeedUserAsync();
        for (int i = 1; i <= 3; i++)
        {
            _context.AuthProviders.Add(new AuthProvider
            {
                Id = Guid.NewGuid(), UserId = user.Id,
                ProviderName = "github", ProviderKey = $"gh_uid_{i}",
                ScopeValidationStatus = ProviderScopeStatus.Valid,
            });
        }
        await _context.SaveChangesAsync();
        SetupUser(user.Id);

        var result = await _sut.ConnectProvider("github");

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    // ── UTCID04 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify16_UTCID04_ConnectGitHub_MissingGitHubClientId_Returns400BadRequest()
    {
        // Override service provider with empty client ID
        var services = new ServiceCollection();
        services.AddDbContext<ApplicationDbContext>(opts =>
            opts.UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning)));
        services.AddSingleton(new EnvConfiguration
        {
            Auth = new AuthSettings { GithubClientId = null }
        });
        await using var spNoId = services.BuildServiceProvider();
        var ctx = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning)).Options);
        var user = new User
        {
            Id = Guid.NewGuid(), Email = "u@e.com", FullName = "U", Username = "u",
            Status = UserStatus.ACTIVE, EmailVerifiedAt = DateTime.UtcNow,
        };
        ctx.Users.Add(user); await ctx.SaveChangesAsync();

        var httpCtx = new DefaultHttpContext { RequestServices = spNoId };
        httpCtx.User = new ClaimsPrincipal(new ClaimsIdentity(
            new[] { new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()) }, "Test"));
        _sut.ControllerContext = new ControllerContext { HttpContext = httpCtx };

        var result = await _sut.ConnectProvider("github");

        result.Should().BeOfType<BadRequestObjectResult>();
        await ctx.DisposeAsync();
    }

    // ── UTCID05 ───────────────────────────────────────────────────────────
    // No JWT → controller finds no NameIdentifier claim → Unauthorized.
    [Fact]
    public async Task CVerify16_UTCID05_ConnectGitHub_NoJwt_Returns401Unauthorized()
    {
        var ctx = new DefaultHttpContext { RequestServices = _sp };
        _sut.ControllerContext = new ControllerContext { HttpContext = ctx };

        var result = await _sut.ConnectProvider("github");

        result.Should().BeOfType<UnauthorizedResult>();
    }

    // ── UTCID06 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify16_UTCID06_ConnectGitHub_UnsupportedProvider_Returns400BadRequest()
    {
        var user = await SeedUserAsync();
        SetupUser(user.Id);

        var result = await _sut.ConnectProvider("twitter");

        result.Should().BeOfType<BadRequestObjectResult>();
    }
}
