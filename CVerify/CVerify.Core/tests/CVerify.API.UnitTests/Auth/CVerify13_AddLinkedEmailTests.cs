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
using CVerify.API.Modules.Auth.DTOs;
using CVerify.API.Modules.Auth.Services;
using CVerify.API.Modules.Shared.Domain.Entities;
using CVerify.API.Modules.Shared.Domain.Enums;
using CVerify.API.Modules.Shared.Exceptions;
using CVerify.API.Modules.Shared.Persistence;

namespace CVerify.API.UnitTests.Auth;

/// <summary>
/// Unit tests for AuthController.VerifyLinkEmailOtp — CVerify-13 (7 UTCIDs).
/// POST /api/auth/emails/verify-otp — verifies OTP and adds a new linked email.
/// </summary>
public sealed class CVerify13_AddLinkedEmailTests : IDisposable
{
    private readonly ServiceProvider _sp;
    private readonly ApplicationDbContext _context;

    private readonly Mock<IAuthService>             _authService           = new();
    private readonly Mock<IIdentityStateResolver>   _identityStateResolver = new();
    private readonly Mock<IWorkspaceProvisioningService> _workspaceProv     = new();

    private readonly AuthController _sut;

    public CVerify13_AddLinkedEmailTests()
    {
        var services = new ServiceCollection();
        services.AddDbContext<ApplicationDbContext>(opts =>
            opts.UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning)));
        _sp = services.BuildServiceProvider();
        _context = _sp.GetRequiredService<ApplicationDbContext>();

        // Default: OTP verification succeeds
        _authService
            .Setup(a => a.VerifyOtpAsync(It.IsAny<VerifyOtpRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new VerifyOtpResponse(Guid.NewGuid(), "new@example.com", "VALID_TOKEN"));
        _authService
            .Setup(a => a.ClaimPendingRelationshipsAsync(It.IsAny<Guid>()))
            .Returns(Task.CompletedTask);
        _identityStateResolver
            .Setup(r => r.InvalidateCacheAsync(It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        _sut = new AuthController(
            _authService.Object,
            _identityStateResolver.Object,
            new Mock<ILogger<AuthController>>().Object,
            _workspaceProv.Object);
    }

    public void Dispose() => _sp.Dispose();

    private void SetupUser(Guid userId)
    {
        var ctx = new DefaultHttpContext { RequestServices = _sp };
        ctx.User = new ClaimsPrincipal(new ClaimsIdentity(
            new[] { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) }, "Test"));
        _sut.ControllerContext = new ControllerContext { HttpContext = ctx };
    }

    private async Task<User> SeedUserAsync(string email = "user@example.com")
    {
        var user = new User
        {
            Id = Guid.NewGuid(), Email = email, FullName = "Test User",
            Username = "testuser", Status = UserStatus.ACTIVE,
            EmailVerifiedAt = DateTime.UtcNow,
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    private static AuthController.VerifyEmailLinkOtpRequest BuildRequest(
        string email = "new@example.com",
        string code = "123456",
        Guid? challengeId = null) =>
        new()
        {
            Email       = email,
            Code        = code,
            ChallengeId = challengeId ?? Guid.NewGuid(),
        };

    // ── UTCID01 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify13_UTCID01_VerifyEmailOtp_ValidCode_Returns200EmailLinked()
    {
        var user = await SeedUserAsync();
        SetupUser(user.Id);

        var result = await _sut.VerifyLinkEmailOtp(BuildRequest("new@example.com", "123456"), CancellationToken.None);

        result.Should().BeOfType<OkObjectResult>();
    }

    // ── UTCID02 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify13_UTCID02_VerifyEmailOtp_WrongCode_Returns400BadRequest()
    {
        var user = await SeedUserAsync();
        SetupUser(user.Id);

        _authService
            .Setup(a => a.VerifyOtpAsync(It.IsAny<VerifyOtpRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new AuthException(AuthErrorCodes.InvalidToken, "OTP code is invalid."));

        var result = await _sut.VerifyLinkEmailOtp(BuildRequest("new@example.com", "999999"), CancellationToken.None);

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    // ── UTCID03 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify13_UTCID03_VerifyEmailOtp_ExpiredChallenge_Returns400BadRequest()
    {
        var user = await SeedUserAsync();
        SetupUser(user.Id);

        _authService
            .Setup(a => a.VerifyOtpAsync(It.IsAny<VerifyOtpRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new AuthException(AuthErrorCodes.InvalidToken, "Challenge has expired."));

        var result = await _sut.VerifyLinkEmailOtp(BuildRequest("new@example.com", "123456"), CancellationToken.None);

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    // ── UTCID04 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify13_UTCID04_VerifyEmailOtp_EmailAlreadyTaken_Returns400BadRequest()
    {
        await SeedUserAsync("taken@example.com");
        var user = await SeedUserAsync("sender@example.com");
        SetupUser(user.Id);

        var result = await _sut.VerifyLinkEmailOtp(BuildRequest("taken@example.com", "123456"), CancellationToken.None);

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    // ── UTCID05 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify13_UTCID05_VerifyEmailOtp_UserAtMaxTwoSecondary_Returns400BadRequest()
    {
        var user = await SeedUserAsync();
        user.LinkedEmails.Add(new LinkedEmail { Id = Guid.NewGuid(), Email = "s1@example.com", IsVerified = true });
        user.LinkedEmails.Add(new LinkedEmail { Id = Guid.NewGuid(), Email = "s2@example.com", IsVerified = true });
        await _context.SaveChangesAsync();
        SetupUser(user.Id);

        var result = await _sut.VerifyLinkEmailOtp(BuildRequest("third@example.com", "123456"), CancellationToken.None);

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    // ── UTCID06 ───────────────────────────────────────────────────────────
    // No JWT → controller finds no NameIdentifier claim → Unauthorized.
    [Fact]
    public async Task CVerify13_UTCID06_VerifyEmailOtp_NoJwt_Returns401Unauthorized()
    {
        var ctx = new DefaultHttpContext { RequestServices = _sp };
        _sut.ControllerContext = new ControllerContext { HttpContext = ctx };

        var result = await _sut.VerifyLinkEmailOtp(BuildRequest(), CancellationToken.None);

        result.Should().BeOfType<UnauthorizedResult>();
    }

    // ── UTCID07 ───────────────────────────────────────────────────────────
    // Email with leading/trailing whitespace and uppercase — controller normalises before checking.
    [Fact]
    public async Task CVerify13_UTCID07_VerifyEmailOtp_EmailWithWhitespaceAndCase_NormalisedAndLinked()
    {
        var user = await SeedUserAsync();
        SetupUser(user.Id);

        var result = await _sut.VerifyLinkEmailOtp(
            BuildRequest("  NEW@EXAMPLE.COM  ", "123456"), CancellationToken.None);

        result.Should().BeOfType<OkObjectResult>("email is trimmed + lowercased before check");
    }
}
