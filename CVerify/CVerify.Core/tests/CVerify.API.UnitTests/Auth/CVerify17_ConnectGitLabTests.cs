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
/// Unit tests for AuthController.ConnectProvider("gitlab") — CVerify-17 (5 UTCIDs).
/// GET /api/auth/connect/gitlab — initiates GitLab OAuth flow.
/// Same pattern as ConnectGitHub (CVerify-16) with gitlab provider name.
/// </summary>
public sealed class CVerify17_ConnectGitLabTests : IDisposable
{
    private readonly ServiceProvider _sp;
    private readonly ApplicationDbContext _context;

    private readonly Mock<IAuthService>                  _authService   = new();
    private readonly Mock<IIdentityStateResolver>        _idResolver    = new();
    private readonly Mock<IWorkspaceProvisioningService> _workspaceProv = new();

    private readonly AuthController _sut;

    public CVerify17_ConnectGitLabTests()
    {
        var services = new ServiceCollection();
        services.AddDbContext<ApplicationDbContext>(opts =>
            opts.UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning)));
        services.AddSingleton(new EnvConfiguration
        {
            Auth = new AuthSettings
            {
                GitlabClientId = "test_gitlab_client_id"
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
    public async Task CVerify17_UTCID01_ConnectGitLab_ValidUser_NoExistingLinks_ReturnsRedirectToGitLab()
    {
        var user = await SeedUserAsync();
        SetupUser(user.Id);

        var result = await _sut.ConnectProvider("gitlab");

        result.Should().BeOfType<RedirectResult>()
            .Which.Url.Should().Contain("gitlab.com/oauth/authorize");
    }

    // ── UTCID02 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify17_UTCID02_ConnectGitLab_UserAlreadyHasThreeLinkedAccounts_Returns400BadRequest()
    {
        var user = await SeedUserAsync();
        for (int i = 1; i <= 3; i++)
        {
            _context.AuthProviders.Add(new AuthProvider
            {
                Id = Guid.NewGuid(), UserId = user.Id,
                ProviderName = "gitlab", ProviderKey = $"gl_uid_{i}",
                ScopeValidationStatus = ProviderScopeStatus.Valid,
            });
        }
        await _context.SaveChangesAsync();
        SetupUser(user.Id);

        var result = await _sut.ConnectProvider("gitlab");

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    // ── UTCID03 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify17_UTCID03_ConnectGitLab_MissingGitLabClientId_Returns400BadRequest()
    {
        var services = new ServiceCollection();
        services.AddDbContext<ApplicationDbContext>(opts =>
            opts.UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning)));
        services.AddSingleton(new EnvConfiguration
        {
            Auth = new AuthSettings { GitlabClientId = null }
        });
        await using var spNoId = services.BuildServiceProvider();
        var ctx2 = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning)).Options);
        var user = new User
        {
            Id = Guid.NewGuid(), Email = "u@e.com", FullName = "U", Username = "u",
            Status = UserStatus.ACTIVE, EmailVerifiedAt = DateTime.UtcNow,
        };
        ctx2.Users.Add(user); await ctx2.SaveChangesAsync();

        var httpCtx = new DefaultHttpContext { RequestServices = spNoId };
        httpCtx.User = new ClaimsPrincipal(new ClaimsIdentity(
            new[] { new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()) }, "Test"));
        _sut.ControllerContext = new ControllerContext { HttpContext = httpCtx };

        var result = await _sut.ConnectProvider("gitlab");

        result.Should().BeOfType<BadRequestObjectResult>();
        await ctx2.DisposeAsync();
    }

    // ── UTCID04 ───────────────────────────────────────────────────────────
    // No JWT → Unauthorized.
    [Fact]
    public async Task CVerify17_UTCID04_ConnectGitLab_NoJwt_Returns401Unauthorized()
    {
        var ctx = new DefaultHttpContext { RequestServices = _sp };
        _sut.ControllerContext = new ControllerContext { HttpContext = ctx };

        var result = await _sut.ConnectProvider("gitlab");

        result.Should().BeOfType<UnauthorizedResult>();
    }

    // ── UTCID05 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify17_UTCID05_ConnectGitLab_UserHasTwoLinkedGitLab_ReturnsRedirect()
    {
        var user = await SeedUserAsync();
        for (int i = 1; i <= 2; i++)
        {
            _context.AuthProviders.Add(new AuthProvider
            {
                Id = Guid.NewGuid(), UserId = user.Id,
                ProviderName = "gitlab", ProviderKey = $"gl_uid_{i}",
                ScopeValidationStatus = ProviderScopeStatus.Valid,
            });
        }
        await _context.SaveChangesAsync();
        SetupUser(user.Id);

        var result = await _sut.ConnectProvider("gitlab");

        result.Should().BeOfType<RedirectResult>("2 of 3 maximum → still allowed");
    }
}
